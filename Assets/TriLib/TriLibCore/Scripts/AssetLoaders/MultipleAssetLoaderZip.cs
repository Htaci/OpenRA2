#pragma warning disable 184

using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using TriLibCore.Mappers;
using TriLibCore.Utils;
using UnityEngine;

namespace TriLibCore
{
    /// <summary>
    /// Provides methods to load every recognized model file within a .zip archive,
    /// leveraging TriLib’s <see cref="AssetLoader"/> under the hood. Unlike <c>AssetLoaderZip</c>,
    /// this class attempts to load multiple models from the same .zip file rather than just the first.
    /// </summary>
    public static class MultipleAssetLoaderZip
    {
        /// <summary>
        /// Loads all valid 3D models from the specified <paramref name="stream"/> (a .zip file stream)
        /// asynchronously if the platform allows threading. Each recognized file extension is
        /// processed individually, generating a separate <see cref="AssetLoaderContext"/>.
        /// </summary>
        /// <remarks>
        /// If multiple models are found, each model is loaded in turn, potentially creating
        /// multiple <see cref="GameObject"/> hierarchies under <paramref name="wrapperGameObject"/>.
        /// </remarks>
        /// <param name="stream">
        /// A <see cref="Stream"/> pointing to the .zip data. The stream is scanned for valid model files (based
        /// on known TriLib extensions).
        /// </param>
        /// <param name="onLoad">
        /// Callback invoked on the main Unity thread once any model’s core data is loaded
        /// (but before materials have fully loaded).
        /// </param>
        /// <param name="onMaterialsLoad">
        /// Callback invoked on the main Unity thread after each model’s materials (textures, shaders, etc.)
        /// have finished loading.
        /// </param>
        /// <param name="onProgress">
        /// Callback invoked whenever any model’s loading progress is updated (0–1). 
        /// Includes both data and materials loading stages.
        /// </param>
        /// <param name="onError">
        /// Callback invoked on the main Unity thread if any error occurs while extracting or loading a model.
        /// This is optional and can be <c>null</c>.
        /// </param>
        /// <param name="wrapperGameObject">
        /// An optional parent <see cref="GameObject"/> to hold the loaded model hierarchies.
        /// If <c>null</c>, each model is created at the root of the scene.
        /// </param>
        /// <param name="assetLoaderOptions">
        /// The <see cref="AssetLoaderOptions"/> controlling how models, textures, and other data
        /// are processed. If <c>null</c>, default settings are created automatically.
        /// </param>
        /// <param name="customContextData">
        /// A custom object or data structure used to store additional information
        /// in each <see cref="AssetLoaderContext"/>. This can be retrieved later via
        /// the loading pipeline.
        /// </param>
        /// <param name="fileExtension">
        /// If specified, only files in the .zip that match this extension (e.g., <c>".fbx"</c>) 
        /// are loaded. If <c>null</c>, any valid TriLib file extension is accepted.
        /// </param>
        /// <param name="haltTask">
        /// If <c>true</c>, creates the loading tasks but does not start them immediately, allowing you
        /// to chain or queue multiple loads manually.
        /// </param>
        /// <param name="onPreLoad">
        /// A callback invoked on a background thread before Unity objects are created (in supported platforms),
        /// useful for advanced setup or logging tasks in parallel.
        /// </param>
        public static void LoadAllModelsFromZipStream(
            Stream stream,
            Action<AssetLoaderContext> onLoad,
            Action<AssetLoaderContext> onMaterialsLoad,
            Action<AssetLoaderContext, float> onProgress,
            Action<IContextualizedError> onError = null,
            GameObject wrapperGameObject = null,
            AssetLoaderOptions assetLoaderOptions = null,
            object customContextData = null,
            string fileExtension = null,
            bool haltTask = false,
            Action<AssetLoaderContext> onPreLoad = null
        )
        {
            if (assetLoaderOptions == null)
            {
                assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
            }
            SetupModelLoading(assetLoaderOptions);
            LoadModelsInternal(
                onLoad,
                onMaterialsLoad,
                onProgress,
                onError,
                wrapperGameObject,
                assetLoaderOptions,
                customContextData,
                fileExtension,
                haltTask,
                onPreLoad,
                stream
            );
        }

        /// <summary>
        /// Loads all valid 3D models from a local .zip file asynchronously if the platform allows threading.
        /// Each recognized file extension within the .zip is processed individually, creating separate
        /// <see cref="AssetLoaderContext"/> instances for each model.
        /// </summary>
        /// <param name="path">
        /// The absolute or relative file path to the .zip archive. If the file doesn’t exist,
        /// an <see cref="Exception"/> is thrown.
        /// </param>
        /// <param name="onLoad">
        /// Callback invoked on the main Unity thread once any model’s core data is loaded
        /// (but before materials fully load).
        /// </param>
        /// <param name="onMaterialsLoad">
        /// Callback invoked after each model’s materials (textures, shaders, etc.) have been loaded.
        /// This occurs on the main Unity thread.
        /// </param>
        /// <param name="onProgress">
        /// Callback invoked whenever any model’s loading progress is updated (a float from 0 to 1).
        /// This can be called multiple times per model load.
        /// </param>
        /// <param name="onError">
        /// Callback invoked if any error occurs while extracting or loading models from the .zip.
        /// Optionally, pass <c>null</c> if no error handling is needed.
        /// </param>
        /// <param name="wrapperGameObject">
        /// An optional parent <see cref="GameObject"/> under which loaded models will be placed.
        /// If <c>null</c>, each model is created at the root scene level.
        /// </param>
        /// <param name="assetLoaderOptions">
        /// The <see cref="AssetLoaderOptions"/> that govern texture loading, material creation,
        /// animations, and other aspects of model import. If <c>null</c>, a default configuration
        /// is used.
        /// </param>
        /// <param name="customContextData">
        /// An optional data object inserted into each <see cref="AssetLoaderContext"/> for custom reference.
        /// </param>
        /// <param name="fileExtension">
        /// A specific file extension (e.g., <c>".gltf"</c>) to limit which models to load from the .zip.
        /// If <c>null</c>, all recognized TriLib model extensions are loaded.
        /// </param>
        /// <param name="haltTask">
        /// If <c>true</c>, creates but does not begin loading tasks, letting you manage or queue them as needed.
        /// </param>
        /// <param name="onPreLoad">
        /// A background-thread callback invoked before Unity objects are generated, suitable for advanced
        /// tasks such as caching or data pre-processing.
        /// </param>
        /// <exception cref="Exception">
        /// Thrown if the <paramref name="path"/> does not point to a valid file.
        /// </exception>
        public static void LoadAllModelsFromZipFile(
            string path,
            Action<AssetLoaderContext> onLoad,
            Action<AssetLoaderContext> onMaterialsLoad,
            Action<AssetLoaderContext, float> onProgress,
            Action<IContextualizedError> onError = null,
            GameObject wrapperGameObject = null,
            AssetLoaderOptions assetLoaderOptions = null,
            object customContextData = null,
            string fileExtension = null,
            bool haltTask = false,
            Action<AssetLoaderContext> onPreLoad = null
        )
        {
            if (!File.Exists(path))
            {
                throw new Exception("File not found");
            }
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            SetupModelLoading(assetLoaderOptions);
            LoadModelsInternal(
                onLoad,
                onMaterialsLoad,
                onProgress,
                onError,
                wrapperGameObject,
                assetLoaderOptions,
                customContextData,
                fileExtension,
                haltTask,
                onPreLoad,
                stream
            );
        }

        /// <summary>
        /// Performs internal setup to ensure <see cref="AssetLoaderOptions"/> contains the
        /// required mappers (<see cref="ZipFileTextureMapper"/> and <see cref="ZipFileExternalDataMapper"/>)
        /// to read from .zip archives.
        /// </summary>
        /// <param name="assetLoaderOptions">
        /// The current loader options, which are updated in-place to include the necessary
        /// zip-based texture and data mappers if not already present.
        /// </param>
        private static void SetupModelLoading(AssetLoaderOptions assetLoaderOptions)
        {
            if (assetLoaderOptions == null)
            {
                assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
            }
            if (!ArrayUtils.ContainsType<ZipFileTextureMapper>(assetLoaderOptions.TextureMappers))
            {
                var textureMapper = ScriptableObject.CreateInstance<ZipFileTextureMapper>();
                if (assetLoaderOptions.TextureMappers == null)
                {
                    assetLoaderOptions.TextureMappers = new TextureMapper[] { textureMapper };
                }
                else
                {
                    ArrayUtils.Add(ref assetLoaderOptions.TextureMappers, textureMapper);
                }
            }
            if (!(assetLoaderOptions.ExternalDataMapper is ZipFileExternalDataMapper))
            {
                assetLoaderOptions.ExternalDataMapper = ScriptableObject.CreateInstance<ZipFileExternalDataMapper>();
            }
        }

        /// <summary>
        /// Iterates through every file in the .zip <see cref="stream"/>. For each entry that matches
        /// a recognized or specified <paramref name="fileExtension"/>, this method extracts the file
        /// into a memory stream and calls <see cref="AssetLoader.LoadModelFromStream"/> to load it.
        /// </summary>
        /// <param name="onLoad">
        /// Callback for when a model’s base data has finished loading.
        /// </param>
        /// <param name="onMaterialsLoad">
        /// Callback for when model materials and resources have finished loading.
        /// </param>
        /// <param name="onProgress">
        /// Callback for ongoing load progress.
        /// </param>
        /// <param name="onError">
        /// Callback if any error occurs.
        /// </param>
        /// <param name="wrapperGameObject">
        /// Optional parent object for all loaded models.
        /// </param>
        /// <param name="assetLoaderOptions">
        /// Inherited or applied loading options from user or defaults.
        /// </param>
        /// <param name="customContextData">
        /// Any user-supplied data to keep track of within <see cref="AssetLoaderContext"/>.
        /// </param>
        /// <param name="fileExtension">
        /// Restricts or filters which entries are treated as valid models.
        /// </param>
        /// <param name="haltTask">
        /// If <c>true</c>, does not start the actual loading task.
        /// </param>
        /// <param name="onPreLoad">
        /// Background-thread callback for advanced setup prior to Unity object creation.
        /// </param>
        /// <param name="stream">
        /// The open <see cref="Stream"/> referencing the .zip data.
        /// </param>
        private static void LoadModelsInternal(
            Action<AssetLoaderContext> onLoad,
            Action<AssetLoaderContext> onMaterialsLoad,
            Action<AssetLoaderContext, float> onProgress,
            Action<IContextualizedError> onError,
            GameObject wrapperGameObject,
            AssetLoaderOptions assetLoaderOptions,
            object customContextData,
            string fileExtension,
            bool haltTask,
            Action<AssetLoaderContext> onPreLoad,
            Stream stream)
        {
            var validExtensions = Readers.Extensions;
            var zipFile = new ZipFile(stream);

            foreach (ZipEntry zipEntry in zipFile)
            {
                if (!zipEntry.IsFile)
                {
                    continue;
                }

                Stream memoryStream = null;
                var checkingFileExtension = FileUtils.GetFileExtension(zipEntry.Name, false);

                // If user-specified extension matches or recognized extension is found
                if (fileExtension != null && checkingFileExtension == fileExtension)
                {
                    memoryStream = AssetLoaderZip.ZipFileEntryToStream(out fileExtension, zipEntry, zipFile);
                }
                else if (validExtensions.Contains(checkingFileExtension))
                {
                    memoryStream = AssetLoaderZip.ZipFileEntryToStream(out fileExtension, zipEntry, zipFile);
                }

                // Add custom context data for zip-based loading
                var customDataDic = (object)CustomDataHelper.CreateCustomDataDictionaryWithData(new ZipLoadCustomContextData
                {
                    ZipFile = zipFile,
                    Stream = stream,
                    OnError = onError,
                    OnMaterialsLoad = onMaterialsLoad
                });
                if (customContextData != null)
                {
                    CustomDataHelper.SetCustomData(ref customDataDic, customContextData);
                }

                if (memoryStream != null)
                {
                    AssetLoader.LoadModelFromStream(
                        memoryStream,
                        zipEntry.Name,
                        fileExtension,
                        onLoad,
                        OnMaterialsLoad,
                        onProgress,
                        OnError,
                        wrapperGameObject,
                        assetLoaderOptions,
                        customDataDic,
                        haltTask,
                        onPreLoad,
                        true
                    );
                }
            }

            // Close the .zip stream if requested in AssetLoaderOptions
            if (assetLoaderOptions.CloseStreamAutomatically)
            {
                stream.Close();
            }
        }

        /// <summary>
        /// Invoked after model materials have finished loading, calling the user’s <c>onMaterialsLoad</c>
        /// delegate if provided. This is attached to each <see cref="AssetLoaderContext"/> to handle final
        /// zip-based cleanup or notifications.
        /// </summary>
        /// <param name="assetLoaderContext">The context referencing loaded model data.</param>
        private static void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
        {
            var zipLoadCustomContextData =
                CustomDataHelper.GetCustomData<ZipLoadCustomContextData>(assetLoaderContext.CustomData);
            if (zipLoadCustomContextData != null)
            {
                // Fire the user’s onMaterialsLoad callback if present
                if (zipLoadCustomContextData.OnMaterialsLoad != null)
                {
                    zipLoadCustomContextData.OnMaterialsLoad(assetLoaderContext);
                }
            }
        }

        /// <summary>
        /// Called when an error occurs in <see cref="AssetLoader"/> while processing zip-based models.
        /// It attempts to close the .zip stream and notifies any user-defined error callback.
        /// </summary>
        /// <param name="contextualizedError">The encountered error, carrying a context if available.</param>
        private static void OnError(IContextualizedError contextualizedError)
        {
            if (contextualizedError?.GetContext() is AssetLoaderContext assetLoaderContext)
            {
                var zipLoadCustomContextData =
                    CustomDataHelper.GetCustomData<ZipLoadCustomContextData>(assetLoaderContext.CustomData);
                if (zipLoadCustomContextData != null)
                {
                    if (zipLoadCustomContextData.Stream != null)
                    {
                        zipLoadCustomContextData.Stream.Close();
                    }
                    if (zipLoadCustomContextData.OnError != null)
                    {
                        zipLoadCustomContextData.OnError.Invoke(contextualizedError);
                    }
                }
            }
        }
    }
}
