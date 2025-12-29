#pragma warning disable 184
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TriLibCore.Mappers;
using TriLibCore.SFB;
using TriLibCore.Utils;
using UnityEngine;

namespace TriLibCore
{
    /// <summary>
    /// Provides functionality for selecting and loading 3D models from file streams (including archives)
    /// and applying them to the Unity scene using TriLib’s <see cref="AssetLoader"/> pipeline.
    /// This class manages callbacks for model loading, progress reporting, and error handling, and it can
    /// optionally destroy itself after the loading process completes.
    /// </summary>
    public class IOAssetLoader : MonoBehaviour
    {
        /// <summary>
        /// Indicates whether this loader component should self-destruct
        /// (destroy its <see cref="GameObject"/>) once model loading completes or fails.
        /// </summary>
        protected bool AutoDestroy;

        /// <summary>
        /// A callback invoked on the main Unity thread after the model’s core data has been loaded
        /// (but potentially before all materials and textures have finished).
        /// </summary>
        protected Action<AssetLoaderContext> OnLoad;

        /// <summary>
        /// A callback invoked on the main Unity thread after all materials, textures, and other
        /// resources have finished loading for the model.
        /// </summary>
        protected Action<AssetLoaderContext> OnMaterialsLoad;

        /// <summary>
        /// A callback triggered whenever the loading progress updates (from 0 to 1). Receives
        /// both the <see cref="AssetLoaderContext"/> and a float progress value.
        /// </summary>
        protected Action<AssetLoaderContext, float> OnProgress;

        /// <summary>
        /// A callback invoked on the main Unity thread if any error occurs during file selection
        /// or loading. Carries an <see cref="IContextualizedError"/> with error details.
        /// </summary>
        protected Action<IContextualizedError> OnError;

        /// <summary>
        /// A callback invoked once file loading begins, passing a <see cref="bool"/> indicating
        /// whether valid files are actually being loaded.
        /// </summary>
        protected Action<bool> OnBeginLoad;

        /// <summary>
        /// An optional parent <see cref="GameObject"/> under which the loaded model hierarchy
        /// will be placed. If <c>null</c>, the model is created at the scene’s root level.
        /// </summary>
        protected GameObject WrapperGameObject;

        /// <summary>
        /// Contains settings that control TriLib’s import behavior, such as material/texture handling,
        /// colliders, animation type, etc. If <c>null</c>, default options are generated.
        /// </summary>
        protected AssetLoaderOptions AssetLoaderOptions;

        /// <summary>
        /// If <c>true</c>, tasks for model loading are created but not started immediately,
        /// allowing the caller to chain or manage them manually.
        /// </summary>
        protected bool HaltTask;

        /// <summary>
        /// A list of <see cref="ItemWithStream"/> objects representing the files selected for loading.
        /// Each item potentially contains a <see cref="System.IO.Stream"/> to the model data.
        /// </summary>
        private IList<ItemWithStream> _items;

        /// <summary>
        /// Stores the file extension of the primary model file (derived via <see cref="FileUtils.GetFileExtension"/>).
        /// Used to determine loading logic (e.g., whether it’s a .zip or recognized 3D format).
        /// </summary>
        private string _modelExtension;

        /// <summary>
        /// Safely destroys the <see cref="GameObject"/> hosting this loader component.
        /// This is called if <see cref="AutoDestroy"/> is <c>true</c> or upon certain
        /// error conditions.
        /// </summary>
        protected void DestroyMe()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Initiates the file loading sequence by starting a coroutine that checks
        /// whether the selected files are valid and proceeds with TriLib loading.
        /// </summary>
        private void HandleFileLoading()
        {
            StartCoroutine(DoHandleFileLoading());
        }

        /// <summary>
        /// The core loading coroutine which verifies file data, triggers TriLib’s
        /// <see cref="AssetLoader"/> or <see cref="AssetLoaderZip"/> (if it's a .zip),
        /// and invokes callbacks for progress, success, or failure.
        /// </summary>
        /// <remarks>
        /// If no valid files are found, the loader may destroy itself if <see cref="AutoDestroy"/> is set.
        /// This method handles both archive loading (via <c>AssetLoaderZip</c>) and direct model
        /// file loading (e.g., <c>FBX</c>, <c>GLTF</c>, etc.).
        /// </remarks>
        private IEnumerator DoHandleFileLoading()
        {
            // Notify that loading is about to begin
            var hasFiles = _items != null && _items.Count > 0 && _items[0].HasData;
            OnBeginLoad?.Invoke(hasFiles);

            // Yield for two frames to let the UI or any other logic update
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            // If no valid files are found, handle cleanup
            if (!hasFiles)
            {
                if (AutoDestroy)
                {
                    DestroyMe();
                }
                yield break;
            }

            // Identify a usable model file from the selected items
            var modelFileWithStream = FindModelFile();
            var modelFilename = modelFileWithStream.Name;
            var modelStream = modelFileWithStream.OpenStream();

            // Create default loader options if none are specified
            if (AssetLoaderOptions == null)
            {
                AssetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(false, true);
            }

            // Ensure FilePicker-based mappers are used if not already present
            if (!ArrayUtils.ContainsType<FilePickerTextureMapper>(AssetLoaderOptions.TextureMappers))
            {
                var textureMapper = ScriptableObject.CreateInstance<FilePickerTextureMapper>();
                if (AssetLoaderOptions.TextureMappers == null)
                {
                    AssetLoaderOptions.TextureMappers = new TextureMapper[] { textureMapper };
                }
                else
                {
                    ArrayUtils.Add(ref AssetLoaderOptions.TextureMappers, textureMapper);
                }
            }
            if (!(AssetLoaderOptions.ExternalDataMapper is FilePickerExternalDataMapper))
            {
                AssetLoaderOptions.ExternalDataMapper = ScriptableObject.CreateInstance<FilePickerExternalDataMapper>();
            }

            // Determine the file extension for loading logic
            _modelExtension = modelFilename != null
                ? FileUtils.GetFileExtension(modelFilename, false)
                : null;

            // If it's a .zip, defer to AssetLoaderZip; otherwise, use normal asset loading
            if (_modelExtension == "zip")
            {
                if (modelStream != null)
                {
                    AssetLoaderZip.LoadModelFromZipStream(
                        modelStream,
                        OnLoad,
                        OnMaterialsLoad,
                        OnProgress,
                        OnError,
                        WrapperGameObject,
                        AssetLoaderOptions,
                        CustomDataHelper.CreateCustomDataDictionaryWithData(_items),
                        null,
                        false,
                        modelFilename
                    );
                }
                else
                {
                    AssetLoaderZip.LoadModelFromZipFile(
                        modelFilename,
                        OnLoad,
                        OnMaterialsLoad,
                        OnProgress,
                        OnError,
                        WrapperGameObject,
                        AssetLoaderOptions,
                        CustomDataHelper.CreateCustomDataDictionaryWithData(_items),
                        null
                    );
                }
            }
            else
            {
                // Load from direct model stream if available, otherwise from file path
                if (modelStream != null)
                {
                    AssetLoader.LoadModelFromStream(
                        modelStream,
                        modelFilename,
                        _modelExtension,
                        OnLoad,
                        OnMaterialsLoad,
                        OnProgress,
                        OnError,
                        WrapperGameObject,
                        AssetLoaderOptions,
                        CustomDataHelper.CreateCustomDataDictionaryWithData(_items),
                        HaltTask
                    );
                }
                else
                {
                    AssetLoader.LoadModelFromFile(
                        modelFilename,
                        OnLoad,
                        OnMaterialsLoad,
                        OnProgress,
                        OnError,
                        WrapperGameObject,
                        AssetLoaderOptions,
                        CustomDataHelper.CreateCustomDataDictionaryWithData(_items),
                        HaltTask
                    );
                }
            }

            // If flagged, destroy this component after the load request
            if (AutoDestroy)
            {
                DestroyMe();
            }
        }

        /// <summary>
        /// Constructs a list of <see cref="ExtensionFilter"/> objects representing valid 3D model
        /// file extensions recognized by TriLib, plus <c>"zip"</c> support. An “All Files” filter 
        /// is also appended for user convenience.
        /// </summary>
        /// <returns>
        /// An array of <see cref="ExtensionFilter"/> used by file-picker dialogs to filter selectable files.
        /// </returns>
        protected static ExtensionFilter[] GetExtensions()
        {
            var extensions = Readers.Extensions;
            var extensionFilters = new List<ExtensionFilter>();
            var subExtensions = new List<string>();

            // Add each recognized TriLib extension to the filters
            for (var i = 0; i < extensions.Count; i++)
            {
                var extension = extensions[i];
                extensionFilters.Add(new ExtensionFilter(null, extension));
                subExtensions.Add(extension);
            }

            // Append zip + an "All Files" wildcard
            subExtensions.Add("zip");
            extensionFilters.Add(new ExtensionFilter(null, new[] { "zip" }));
            extensionFilters.Add(new ExtensionFilter("All Files", new[] { "*" }));

            // Insert a combined filter at the top for convenience
            extensionFilters.Insert(0, new ExtensionFilter("Accepted Files", subExtensions.ToArray()));
            return extensionFilters.ToArray();
        }

        /// <summary>
        /// Finds the first model file (or any recognized 3D file extension) from the
        /// <see cref="_items"/> list. If exactly one file is selected, that file is returned,
        /// even if its extension is unrecognized.
        /// </summary>
        /// <returns>
        /// An <see cref="ItemWithStream"/> corresponding to the primary model file, or <c>null</c>
        /// if no recognized model files are found.
        /// </returns>
        private ItemWithStream FindModelFile()
        {
            // If only one file is present, choose it directly
            if (_items.Count == 1)
            {
                return _items.First();
            }

            // Otherwise, scan for recognized TriLib model file extensions
            var extensions = Readers.Extensions;
            for (var i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                if (item.Name == null)
                {
                    continue;
                }

                var extension = FileUtils.GetFileExtension(item.Name, false);
                if (extensions.Contains(extension))
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Called once the user has selected or provided streams to load. If valid streams are present,
        /// this method triggers file loading via <see cref="HandleFileLoading"/>. Otherwise, it performs
        /// cleanup if <see cref="AutoDestroy"/> is enabled.
        /// </summary>
        /// <param name="itemsWithStream">
        /// A list of file data items (each containing a <see cref="System.IO.Stream"/>) for model loading.
        /// May be <c>null</c> or empty if the user canceled or no data is available.
        /// </param>
        protected void OnItemsWithStreamSelected(IList<ItemWithStream> itemsWithStream)
        {
            if (itemsWithStream != null)
            {
                _items = itemsWithStream;
                Dispatcher.InvokeAsync(HandleFileLoading);
            }
            else
            {
                if (AutoDestroy)
                {
                    DestroyMe();
                }
            }
        }
    }
}
