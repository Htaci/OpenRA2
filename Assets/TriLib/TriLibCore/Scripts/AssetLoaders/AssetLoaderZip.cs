#pragma warning disable 184

using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using TriLibCore.General;
using TriLibCore.Mappers;
using TriLibCore.Utils;
using UnityEngine;

namespace TriLibCore
{
    /// <summary>
    /// Provides methods to load 3D models from a .zip file or stream using TriLib. It wraps around
    /// the core <see cref="AssetLoader"/> methods, automatically unpacking the first suitable 3D 
    /// model file found within the archive and configuring any external data mappers necessary 
    /// for successful loading of textures, materials, and other resources.
    ///
    /// <para>
    /// Typical use involves calling one of the public methods (e.g., <see cref="LoadModelFromZipFile"/> 
    /// or <see cref="LoadModelFromZipStream"/>) to load a zipped model either synchronously or asynchronously.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Note that these methods rely on TriLib’s <see cref="AssetLoader"/> for the final stage of 
    /// model loading and may incorporate <see cref="ZipFileTextureMapper"/> and 
    /// <see cref="ZipFileExternalDataMapper"/> to handle external textures and data.  
    /// </remarks>
    public static class AssetLoaderZip
    {
        /// <summary>
        /// The size of the buffer (in bytes) used when copying zip entry contents to memory.
        /// </summary>
        private const int ZipBufferSize = 4096;

        /// <summary>
        /// Invoked internally when all model resources (e.g., materials, textures) have finished loading.
        /// Closes the <see cref="Stream"/> associated with the .zip archive and calls any user-supplied 
        /// <c>OnMaterialsLoad</c> callback.
        /// </summary>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> containing callbacks and model loading information.
        /// </param>
        private static void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
        {
            var zipLoadCustomContextData = CustomDataHelper.GetCustomData<ZipLoadCustomContextData>(assetLoaderContext.CustomData);
            if (zipLoadCustomContextData != null)
            {
                if (zipLoadCustomContextData.Stream != null)
                {
                    zipLoadCustomContextData.Stream.Close();
                }

                // Invoke the original onMaterialsLoad callback if provided
                if (zipLoadCustomContextData.OnMaterialsLoad != null)
                {
                    zipLoadCustomContextData.OnMaterialsLoad(assetLoaderContext);
                }
            }
        }

        /// <summary>
        /// Invoked when an error occurs during loading. Closes the .zip <see cref="Stream"/> 
        /// and triggers any user-supplied error callback.
        /// </summary>
        /// <param name="contextualizedError">
        /// The error that was thrown, potentially containing contextual loading information.
        /// </param>
        private static void OnError(IContextualizedError contextualizedError)
        {
            if (contextualizedError?.GetContext() is AssetLoaderContext assetLoaderContext)
            {
                var zipLoadCustomContextData = CustomDataHelper.GetCustomData<ZipLoadCustomContextData>(assetLoaderContext.CustomData);
                if (zipLoadCustomContextData != null)
                {
                    if (zipLoadCustomContextData.Stream != null)
                    {
                        zipLoadCustomContextData.Stream.Close();
                    }

                    // Invoke the original onError callback if provided
                    zipLoadCustomContextData.OnError?.Invoke(contextualizedError);
                }
            }
        }

        /// <summary>
        /// Loads the first recognized model file found within the specified .zip archive 
        /// asynchronously (on a separate thread if supported) and returns the resulting
        /// <see cref="AssetLoaderContext"/>.
        /// </summary>
        /// <param name="path">The file path pointing to the .zip archive.</param>
        /// <param name="onLoad">
        /// Callback invoked on the main thread once the model is loaded (but materials 
        /// may still be pending).
        /// </param>
        /// <param name="onMaterialsLoad">
        /// Callback invoked on the main thread after all materials (textures, shaders, etc.) 
        /// have finished loading.
        /// </param>
        /// <param name="onProgress">
        /// Callback invoked when loading progress changes. Accepts an <see cref="AssetLoaderContext"/> 
        /// and a float in the range [0, 1].
        /// </param>
        /// <param name="onError">
        /// Callback invoked on the main thread if any error occurs during .zip extraction 
        /// or model loading.
        /// </param>
        /// <param name="wrapperGameObject">
        /// Optional parent <see cref="GameObject"/> for the resulting model hierarchy.
        /// If <c>null</c>, the model loads at the root scene level.
        /// </param>
        /// <param name="assetLoaderOptions">
        /// The <see cref="AssetLoaderOptions"/> controlling how the model is loaded 
        /// (e.g., material/texture mappers, animation settings).
        /// </param>
        /// <param name="customContextData">
        /// User-defined data to store in the <see cref="AssetLoaderContext"/> 
        /// for customization or reference during loading.
        /// </param>
        /// <param name="fileExtension">
        /// Optional. If known, the file extension of the model inside the .zip (e.g., <c>".fbx"</c>).
        /// If <c>null</c>, TriLib attempts to find any recognized 3D file format in the archive.
        /// </param>
        /// <param name="haltTask">
        /// If <c>true</c>, the loading tasks are created but not started immediately, 
        /// allowing for manual chaining or scheduling of multiple loads.
        /// </param>
        /// <param name="onPreLoad">
        /// Callback invoked on a background thread before any Unity objects are instantiated.
        /// This is useful for performing setup tasks in parallel.
        /// </param>
        /// <returns>
        /// An <see cref="AssetLoaderContext"/> containing references to the loaded model hierarchy 
        /// and other loading details. Returns <c>null</c> if no suitable model was found in the .zip.
        /// </returns>
        public static AssetLoaderContext LoadModelFromZipFile(
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
            Action<AssetLoaderContext> onPreLoad = null)
        {
            Stream stream = null;
            var memoryStream = SetupZipModelLoading(
                onError,
                ref stream,
                path,
                ref assetLoaderOptions,
                ref fileExtension,
                out var zipFile,
                out var zipEntry
            );

            // Create a custom data dictionary that references the .zip file/entry
            var customDataDic = (object)CustomDataHelper.CreateCustomDataDictionaryWithData(
                new ZipLoadCustomContextData
                {
                    ZipFile = zipFile,
                    ZipEntry = zipEntry,
                    Stream = stream,
                    OnError = onError,
                    OnMaterialsLoad = onMaterialsLoad
                }
            );

            // Merge any user-supplied custom context data
            if (customContextData != null)
            {
                CustomDataHelper.SetCustomData(ref customDataDic, customContextData);
            }

            // If no suitable model was found, memoryStream is null
            return memoryStream == null
                ? null
                : AssetLoader.LoadModelFromStream(
                    memoryStream,
                    path,
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

        /// <summary>
        /// Loads the first recognized model file found in the given .zip data <see cref="Stream"/>
        /// asynchronously and returns the <see cref="AssetLoaderContext"/>.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> containing the .zip data.</param>
        /// <param name="onLoad">
        /// Callback invoked on the main thread once the model is loaded (but materials 
        /// may still be pending).
        /// </param>
        /// <param name="onMaterialsLoad">
        /// Callback invoked on the main thread after all materials (textures, shaders, etc.) 
        /// have finished loading.
        /// </param>
        /// <param name="onProgress">
        /// Callback invoked to report loading progress, from 0 to 1.
        /// </param>
        /// <param name="onError">
        /// Callback invoked on the main thread if any error occurs during .zip extraction 
        /// or model loading.
        /// </param>
        /// <param name="wrapperGameObject">
        /// Optional parent <see cref="GameObject"/> for the resulting model hierarchy.
        /// If <c>null</c>, the model loads at the root scene level.
        /// </param>
        /// <param name="assetLoaderOptions">
        /// The <see cref="AssetLoaderOptions"/> controlling how the model is loaded.
        /// </param>
        /// <param name="customContextData">
        /// User-defined data stored in the <see cref="AssetLoaderContext"/> for reference.
        /// </param>
        /// <param name="fileExtension">
        /// Optional. The file extension of the model inside the .zip. If <c>null</c>, 
        /// TriLib attempts to locate a recognized 3D format automatically.
        /// </param>
        /// <param name="haltTask">
        /// If <c>true</c>, the loading tasks are created but not started immediately.
        /// </param>
        /// <param name="modelFilename">
        /// An optional filename to associate with the model in the <see cref="AssetLoaderContext"/>. 
        /// This is useful for logging or reference within TriLib if the .zip data was sourced 
        /// from a file path.
        /// </param>
        /// <param name="onPreLoad">
        /// A callback invoked on a background thread before any Unity objects are instantiated,
        /// allowing parallel setup tasks.
        /// </param>
        /// <returns>
        /// An <see cref="AssetLoaderContext"/> providing references to the loaded model 
        /// and additional data. Returns <c>null</c> if no suitable model is found in the .zip.
        /// </returns>
        public static AssetLoaderContext LoadModelFromZipStream(
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
            string modelFilename = null,
            Action<AssetLoaderContext> onPreLoad = null)
        {
            var memoryStream = SetupZipModelLoading(
                onError,
                ref stream,
                null,
                ref assetLoaderOptions,
                ref fileExtension,
                out var zipFile,
                out var zipEntry
            );

            var customDataDic = (object)CustomDataHelper.CreateCustomDataDictionaryWithData(
                new ZipLoadCustomContextData
                {
                    ZipFile = zipFile,
                    ZipEntry = zipEntry,
                    Stream = stream,
                    OnError = onError,
                    OnMaterialsLoad = onMaterialsLoad
                }
            );

            if (customContextData != null)
            {
                CustomDataHelper.SetCustomData(ref customDataDic, customContextData);
            }

            return AssetLoader.LoadModelFromStream(
                memoryStream,
                modelFilename,
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

        /// <summary>
        /// Loads the first recognized model file from the specified .zip archive synchronously 
        /// on the main thread. This method does not utilize background threads or coroutines.
        /// </summary>
        /// <param name="path">The file path pointing to the .zip archive.</param>
        /// <param name="onError">
        /// Callback invoked on the main thread if any error occurs during extraction 
        /// or model loading.
        /// </param>
        /// <param name="wrapperGameObject">
        /// Optional parent <see cref="GameObject"/> for the resulting model hierarchy.
        /// </param>
        /// <param name="assetLoaderOptions">
        /// The <see cref="AssetLoaderOptions"/> controlling how the model is loaded.
        /// </param>
        /// <param name="customContextData">
        /// Additional user-defined data stored in the <see cref="AssetLoaderContext"/>.
        /// </param>
        /// <param name="fileExtension">
        /// Optional. The file extension of the model inside the .zip. If <c>null</c>, 
        /// TriLib attempts to locate a recognized 3D format automatically.
        /// </param>
        /// <returns>
        /// An <see cref="AssetLoaderContext"/> referencing the loaded model and related data.
        /// Returns <c>null</c> if no valid model was found in the .zip.
        /// </returns>
        public static AssetLoaderContext LoadModelFromZipFileNoThread(
            string path,
            Action<IContextualizedError> onError = null,
            GameObject wrapperGameObject = null,
            AssetLoaderOptions assetLoaderOptions = null,
            object customContextData = null,
            string fileExtension = null)
        {
            Stream stream = null;
            var memoryStream = SetupZipModelLoading(
                onError,
                ref stream,
                path,
                ref assetLoaderOptions,
                ref fileExtension,
                out var zipFile,
                out var zipEntry
            );

            var customDataDic = (object)CustomDataHelper.CreateCustomDataDictionaryWithData(
                new ZipLoadCustomContextData
                {
                    ZipFile = zipFile,
                    ZipEntry = zipEntry,
                    Stream = stream,
                    OnError = onError
                }
            );

            if (customContextData != null)
            {
                CustomDataHelper.SetCustomData(ref customDataDic, customContextData);
            }

            var assetLoaderContext = AssetLoader.LoadModelFromStreamNoThread(
                memoryStream,
                path,
                fileExtension,
                OnError,
                wrapperGameObject,
                assetLoaderOptions,
                customDataDic,
                true
            );

            // Close the underlying .zip stream once loading completes
            stream.Close();
            return assetLoaderContext;
        }

        /// <summary>
        /// Loads the first recognized model file from the specified .zip data <see cref="Stream"/>
        /// synchronously on the main thread. No background threading is used.
        /// </summary>
        /// <param name="stream">
        /// A <see cref="Stream"/> containing the .zip data.
        /// </param>
        /// <param name="onError">
        /// Callback invoked on the main thread if any error occurs during extraction 
        /// or model loading.
        /// </param>
        /// <param name="wrapperGameObject">
        /// Optional parent <see cref="GameObject"/> for the resulting model hierarchy.
        /// </param>
        /// <param name="assetLoaderOptions">
        /// The <see cref="AssetLoaderOptions"/> controlling how the model is loaded.
        /// </param>
        /// <param name="customContextData">
        /// Additional user-defined data stored in the <see cref="AssetLoaderContext"/>.
        /// </param>
        /// <param name="fileExtension">
        /// Optional. The file extension of the model inside the .zip. If <c>null</c>, 
        /// TriLib attempts to locate a recognized 3D format automatically.
        /// </param>
        /// <returns>
        /// An <see cref="AssetLoaderContext"/> referencing the loaded model and related data.
        /// </returns>
        public static AssetLoaderContext LoadModelFromZipStreamNoThread(
            Stream stream,
            Action<IContextualizedError> onError,
            GameObject wrapperGameObject = null,
            AssetLoaderOptions assetLoaderOptions = null,
            object customContextData = null,
            string fileExtension = null)
        {
            var memoryStream = SetupZipModelLoading(
                onError,
                ref stream,
                null,
                ref assetLoaderOptions,
                ref fileExtension,
                out var zipFile,
                out var zipEntry
            );

            var customDataDic = (object)CustomDataHelper.CreateCustomDataDictionaryWithData(
                new ZipLoadCustomContextData
                {
                    ZipFile = zipFile,
                    ZipEntry = zipEntry,
                    Stream = stream,
                    OnError = onError
                }
            );

            if (customContextData != null)
            {
                CustomDataHelper.SetCustomData(ref customDataDic, customContextData);
            }

            var assetLoaderContext = AssetLoader.LoadModelFromStreamNoThread(
                memoryStream,
                null,
                fileExtension,
                OnError,
                wrapperGameObject,
                assetLoaderOptions,
                customDataDic,
                true
            );

            // Close the underlying .zip stream once loading completes
            stream.Close();
            return assetLoaderContext;
        }

        /// <summary>
        /// Configures the <see cref="AssetLoaderOptions"/> for .zip-based loading, injecting 
        /// mappers (e.g., <see cref="ZipFileTextureMapper"/>) needed to handle external data 
        /// within the archive, and extracts the first recognized model file into a memory stream.
        /// </summary>
        /// <param name="onError">Callback invoked if an error occurs during the process.</param>
        /// <param name="stream">
        /// A reference to a <see cref="Stream"/> that will be opened or assigned if <paramref name="path"/> is provided.
        /// </param>
        /// <param name="path">
        /// An optional file path for reading the .zip data. If <c>null</c>, <paramref name="stream"/> 
        /// is assumed to be already open.
        /// </param>
        /// <param name="assetLoaderOptions">
        /// The current <see cref="AssetLoaderOptions"/>, which may be updated to include 
        /// <see cref="ZipFileTextureMapper"/> or <see cref="ZipFileExternalDataMapper"/>.
        /// </param>
        /// <param name="fileExtension">
        /// An optional file extension filter for the model(s) inside the .zip. 
        /// If <c>null</c>, the method tries to find any recognized 3D format.
        /// </param>
        /// <param name="zipFile">Outputs the <see cref="ZipFile"/> object representing the archive.</param>
        /// <param name="modelZipEntry">Outputs the <see cref="ZipEntry"/> of the first recognized model file.</param>
        /// <returns>
        /// A <see cref="Stream"/> containing the unzipped model data (in memory), or <c>null</c> 
        /// if no suitable model file was found.
        /// </returns>
        private static Stream SetupZipModelLoading(
            Action<IContextualizedError> onError,
            ref Stream stream,
            string path,
            ref AssetLoaderOptions assetLoaderOptions,
            ref string fileExtension,
            out ZipFile zipFile,
            out ZipEntry modelZipEntry)
        {
            // Ensure AssetLoaderOptions is set and inject needed mappers for .zip loading
            if (assetLoaderOptions == null)
            {
                assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
            }

            // Add ZipFileTextureMapper if not already present
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

            // Set or replace the external data mapper to handle .zip archives
            if (!(assetLoaderOptions.ExternalDataMapper is ZipFileExternalDataMapper))
            {
                assetLoaderOptions.ExternalDataMapper = ScriptableObject.CreateInstance<ZipFileExternalDataMapper>();
            }

            // If a path is given, open a file stream; otherwise assume we have an existing stream
            if (stream == null)
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            // Determine recognized TriLib extensions
            var validExtensions = Readers.Extensions;

            zipFile = new ZipFile(stream);
            modelZipEntry = null;
            Stream memoryStream = null;

            // Search the .zip for the first recognized model extension
            foreach (ZipEntry zipEntry in zipFile)
            {
                if (!zipEntry.IsFile)
                {
                    continue;
                }

                var checkingFileExtension = FileUtils.GetFileExtension(zipEntry.Name, false);

                // If user provided a file extension or we detect it from the entry name
                if (fileExtension != null && checkingFileExtension == fileExtension)
                {
                    memoryStream = ZipFileEntryToStream(out fileExtension, zipEntry, zipFile);
                    modelZipEntry = zipEntry;
                }
                else if (validExtensions.Contains(checkingFileExtension))
                {
                    memoryStream = ZipFileEntryToStream(out fileExtension, zipEntry, zipFile);
                    modelZipEntry = zipEntry;
                    break;
                }
            }

            // If we still haven't found a suitable model, report an error
            if (memoryStream == null)
            {
                var exception = new Exception(
                    "Unable to find a suitable model within the .zip file. " +
                    "Please specify a valid model file extension."
                );
                onError?.Invoke(new ContextualizedError<string>(exception, "Error"));
            }

            return memoryStream;
        }

        /// <summary>
        /// Extracts the contents of a given <see cref="ZipEntry"/> into a <see cref="MemoryStream"/>. 
        /// The <paramref name="fileExtension"/> is determined from the entry’s filename.
        /// </summary>
        /// <param name="fileExtension">Outputs the extension of the entry’s file (e.g., <c>".fbx"</c>).</param>
        /// <param name="zipEntry">The <see cref="ZipEntry"/> representing a single file in the archive.</param>
        /// <param name="zipFile">The <see cref="ZipFile"/> containing the <paramref name="zipEntry"/>.</param>
        /// <returns>
        /// A <see cref="MemoryStream"/> containing the uncompressed data of the <paramref name="zipEntry"/>,
        /// rewound to the beginning for immediate reading.
        /// </returns>
        public static Stream ZipFileEntryToStream(out string fileExtension, ZipEntry zipEntry, ZipFile zipFile)
        {
            var buffer = new byte[ZipBufferSize];
            var zipFileStream = zipFile.GetInputStream(zipEntry);

            // Allocate a memory stream and copy data from the archive
            var memoryStream = new MemoryStream(ZipBufferSize);
            ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(zipFileStream, memoryStream, buffer);

            // Reset stream position to start
            memoryStream.Seek(0, SeekOrigin.Begin);
            zipFileStream.Close();

            // Determine the file extension from the zip entry name
            fileExtension = FileUtils.GetFileExtension(zipEntry.Name, false);
            return memoryStream;
        }
    }
}
