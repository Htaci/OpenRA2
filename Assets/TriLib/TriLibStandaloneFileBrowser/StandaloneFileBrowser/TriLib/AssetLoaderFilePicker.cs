#pragma warning disable 618

using System;
using TriLibCore.General;
using TriLibCore.SFB;
using TriLibCore.Utils;
using UnityEngine;

namespace TriLibCore
{
    /// <summary>
    /// Extends <see cref="IOAssetLoader"/> to handle model file or directory selection using 
    /// a platform-specific file picker. Once a user selects one or more files/folders, 
    /// TriLib's loading pipeline is triggered to import the models (including materials, textures, etc.).
    /// </summary>
    public class AssetLoaderFilePicker : IOAssetLoader
    {
        /// <summary>
        /// Creates a new <see cref="AssetLoaderFilePicker"/> singleton instance attached
        /// to a <see cref="GameObject"/> in the current scene. By default,
        /// <see cref="AutoDestroy"/> is enabled, causing the component to self-destruct 
        /// once loading completes or fails.
        /// </summary>
        /// <returns>
        /// A reference to the newly instantiated <see cref="AssetLoaderFilePicker"/>.
        /// </returns>
        public static AssetLoaderFilePicker Create()
        {
            var gameObject = new GameObject("AssetLoaderFilePicker");
            var assetLoaderFilePicker = gameObject.AddComponent<AssetLoaderFilePicker>();
            assetLoaderFilePicker.AutoDestroy = true;
            return assetLoaderFilePicker;
        }

        /// <summary>
        /// Opens an OS-native file selection dialog asynchronously, allowing the user
        /// to pick one or more model files. TriLib then loads the selected model(s) 
        /// in the background, invoking the provided callbacks at each stage 
        /// (e.g., initial load, material load, progress, error).
        /// </summary>
        /// <param name="title">The title displayed in the file dialog window.</param>
        /// <param name="onLoad">
        /// A callback invoked on the main thread once the model is loaded (but possibly 
        /// before all materials have been processed).
        /// </param>
        /// <param name="onMaterialsLoad">
        /// A callback invoked on the main thread after the model’s materials 
        /// (textures, shaders, etc.) are fully loaded.
        /// </param>
        /// <param name="onProgress">
        /// A callback that receives updates on the model loading progress (0 to 1).
        /// </param>
        /// <param name="onBeginLoad">
        /// A callback invoked when the loading process begins, passing a <c>bool</c> 
        /// indicating whether any valid files were selected.
        /// </param>
        /// <param name="onError">
        /// A callback invoked if any error occurs during the file selection or loading process, 
        /// providing an <see cref="IContextualizedError"/> for debugging or user messaging.
        /// </param>
        /// <param name="wrapperGameObject">
        /// An optional parent <see cref="GameObject"/> under which the loaded model(s) 
        /// will be placed. If <c>null</c>, the models are created at the root scene level.
        /// </param>
        /// <param name="assetLoaderOptions">
        /// A set of <see cref="AssetLoaderOptions"/> that govern loading behavior 
        /// (e.g., whether to import lights, animations, colliders, etc.). If <c>null</c>, 
        /// TriLib default options are used.
        /// </param>
        /// <param name="haltTask">
        /// If <c>true</c>, TriLib prepares but does not immediately start loading tasks,
        /// letting you schedule or chain them manually.
        /// </param>
        /// <remarks>
        /// This method calls <see cref="OnItemsWithStreamSelected"/> once the user finishes selecting files,
        /// which triggers the actual load process through TriLib’s pipeline.
        /// </remarks>
        public void LoadModelFromFilePickerAsync(
            string title,
            Action<AssetLoaderContext> onLoad = null,
            Action<AssetLoaderContext> onMaterialsLoad = null,
            Action<AssetLoaderContext, float> onProgress = null,
            Action<bool> onBeginLoad = null,
            Action<IContextualizedError> onError = null,
            GameObject wrapperGameObject = null,
            AssetLoaderOptions assetLoaderOptions = null,
            bool haltTask = false
        )
        {
            OnLoad = onLoad;
            OnMaterialsLoad = onMaterialsLoad;
            OnProgress = onProgress;
            OnError = onError;
            OnBeginLoad = onBeginLoad;
            WrapperGameObject = wrapperGameObject;
            AssetLoaderOptions = assetLoaderOptions;
            HaltTask = haltTask;
            try
            {
                // This method displays the system’s file dialog, filtering accepted file types
                StandaloneFileBrowser.OpenFilePanelAsync(title, null, GetExtensions(), true, OnItemsWithStreamSelected);
            }
            catch (Exception e)
            {
                Dispatcher.InvokeAsync(DestroyMe);
                OnError(new ContextualizedError<object>(e, null));
            }
        }

        /// <summary>
        /// Opens an OS-native folder selection dialog asynchronously, allowing the user
        /// to pick one or more directories containing model files. TriLib then scans 
        /// and loads any recognized models in the background, invoking the provided callbacks 
        /// for load completion, material loading, progress, or errors.
        /// </summary>
        /// <param name="title">The title displayed in the folder dialog window.</param>
        /// <param name="onLoad">
        /// A callback invoked on the main thread once each model is loaded (before 
        /// its materials may be fully processed).
        /// </param>
        /// <param name="onMaterialsLoad">
        /// A callback invoked on the main thread after each model’s materials (textures, shaders, etc.) 
        /// are fully loaded.
        /// </param>
        /// <param name="onProgress">
        /// A callback reporting the model loading progress (range 0–1).
        /// </param>
        /// <param name="onBeginLoad">
        /// A callback invoked when loading starts, with a <c>bool</c> indicating whether
        /// any valid directories or files were selected.
        /// </param>
        /// <param name="onError">
        /// A callback invoked if an error occurs during directory selection or loading, 
        /// supplying an <see cref="IContextualizedError"/> for logging or UI feedback.
        /// </param>
        /// <param name="wrapperGameObject">
        /// An optional parent <see cref="GameObject"/> under which loaded models will be placed. 
        /// If <c>null</c>, the models are created at the scene’s root.
        /// </param>
        /// <param name="assetLoaderOptions">
        /// A set of <see cref="AssetLoaderOptions"/> that control the import process. If <c>null</c>, 
        /// TriLib’s default options are used.
        /// </param>
        /// <param name="haltTask">
        /// If <c>true</c>, TriLib queues but does not initiate loading tasks immediately, 
        /// enabling manual scheduling or chaining of loads.
        /// </param>
        /// <remarks>
        /// Similar to <see cref="LoadModelFromFilePickerAsync"/> but uses a folder selection UI 
        /// and attempts to load any recognized 3D files found in the chosen directories.
        /// </remarks>
        public void LoadModelFromDirectoryPickerAsync(
            string title,
            Action<AssetLoaderContext> onLoad = null,
            Action<AssetLoaderContext> onMaterialsLoad = null,
            Action<AssetLoaderContext, float> onProgress = null,
            Action<bool> onBeginLoad = null,
            Action<IContextualizedError> onError = null,
            GameObject wrapperGameObject = null,
            AssetLoaderOptions assetLoaderOptions = null,
            bool haltTask = false
        )
        {
            OnLoad = onLoad;
            OnMaterialsLoad = onMaterialsLoad;
            OnProgress = onProgress;
            OnError = onError;
            OnBeginLoad = onBeginLoad;
            WrapperGameObject = wrapperGameObject;
            AssetLoaderOptions = assetLoaderOptions;
            HaltTask = haltTask;
            try
            {
                // This method displays the system’s folder dialog, scanning the selected directory for models
                StandaloneFileBrowser.OpenFolderPanelAsync(title, null, true, OnItemsWithStreamSelected);
            }
            catch (Exception e)
            {
                Dispatcher.InvokeAsync(DestroyMe);
                OnError(new ContextualizedError<object>(e, null));
            }
        }
    }
}
