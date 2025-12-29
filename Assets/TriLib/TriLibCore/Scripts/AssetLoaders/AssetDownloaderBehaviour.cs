using System;
using System.Collections;
using System.IO;
using TriLibCore.General;
using TriLibCore.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace TriLibCore
{
    /// <summary>
    /// Provides coroutines for asynchronously downloading model data from a remote source,
    /// then passing that data to TriLib’s <see cref="AssetLoader"/> or <see cref="AssetLoaderZip"/> 
    /// for actual model loading. This component is spawned at runtime by 
    /// <see cref="AssetDownloader.LoadModelFromUri"/> and automatically destroyed once 
    /// the download and loading processes finish.
    /// </summary>
    public class AssetDownloaderBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Stores the <see cref="UnityWebRequest"/> used to download the model data.
        /// </summary>
        private UnityWebRequest _unityWebRequest;

        /// <summary>
        /// References the callback method that is invoked to report loading progress.
        /// The callback receives an <see cref="AssetLoaderContext"/> and a <see cref="float"/> 
        /// representing the current download or loading progress.
        /// </summary>
        private Action<AssetLoaderContext, float> _onProgress;

        /// <summary>
        /// Holds the <see cref="AssetLoaderContext"/> created after the model begins loading,
        /// allowing further tracking or modification of the load state.
        /// </summary>
        private AssetLoaderContext _assetLoaderContext;

        /// <summary>
        /// Downloads the model data via the given <paramref name="unityWebRequest"/>, then 
        /// hands off the data to the appropriate loading method (<see cref="AssetLoader.LoadModelFromStream"/> 
        /// or <see cref="AssetLoaderZip.LoadModelFromZipStream"/>) based on file extension or 
        /// content type. Once completed, this <see cref="GameObject"/> is destroyed.
        /// </summary>
        /// <param name="unityWebRequest">
        /// The <see cref="UnityWebRequest"/> object prepared for downloading the model data.
        /// </param>
        /// <param name="onLoad">
        /// A callback invoked on the main thread once the core model data has been processed 
        /// (but before materials are fully loaded).
        /// </param>
        /// <param name="onMaterialsLoad">
        /// A callback invoked on the main thread once the model’s materials (textures, shaders, etc.) 
        /// have finished loading.
        /// </param>
        /// <param name="onProgress">
        /// A callback invoked throughout the download and loading process, indicating progress 
        /// with a <c>float</c> in the range [0, 1].
        /// </param>
        /// <param name="wrapperGameObject">
        /// An optional parent <see cref="GameObject"/> under which the loaded model's 
        /// <see cref="GameObject"/> is placed. Can be <c>null</c> for a root-level object.
        /// </param>
        /// <param name="onError">
        /// A callback invoked if any error occurs during download or loading. Called on the main thread.
        /// </param>
        /// <param name="assetLoaderOptions">
        /// The <see cref="AssetLoaderOptions"/> controlling how the model is loaded (e.g., materials, textures, animations).
        /// </param>
        /// <param name="customContextData">
        /// Optional custom data that can be stored in the <see cref="AssetLoaderContext"/> 
        /// for reference in the loading pipeline.
        /// </param>
        /// <param name="fileExtension">
        /// The file extension for the model (e.g., <c>".fbx"</c>), if known. If the file is 
        /// recognized as a .zip, this may be used for the internal file within the archive.
        /// </param>
        /// <param name="isZipFile">
        /// A boolean (or <c>null</c>) indicating whether the file is a .zip archive. If <c>null</c>, 
        /// the method infers this based on the <c>Content-Type</c> header or <paramref name="fileExtension"/>.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerator"/> that can be yielded in a Unity coroutine, controlling 
        /// the download process. Once the download completes and loading is triggered, this
        /// object destroys itself via <see cref="Object.Destroy"/>.
        /// </returns>
        public IEnumerator DownloadAsset(
            UnityWebRequest unityWebRequest,
            Action<AssetLoaderContext> onLoad,
            Action<AssetLoaderContext> onMaterialsLoad,
            Action<AssetLoaderContext, float> onProgress,
            GameObject wrapperGameObject,
            Action<IContextualizedError> onError,
            AssetLoaderOptions assetLoaderOptions,
            object customContextData,
            string fileExtension,
            bool? isZipFile = null)
        {
            _unityWebRequest = unityWebRequest;
            _onProgress = onProgress;

            // Begin the download request and wait for it to finish
            yield return unityWebRequest.SendWebRequest();

            try
            {
                // Check if the HTTP response indicates success
                if (unityWebRequest.responseCode < 400)
                {
                    // Convert the downloaded data to a MemoryStream for model loading
                    var memoryStream = new MemoryStream(_unityWebRequest.downloadHandler.data);

                    // Create a custom data dictionary with the UnityWebRequest context
                    var customDataDic = (object)CustomDataHelper.CreateCustomDataDictionaryWithData(
                        new UriLoadCustomContextData
                        {
                            UnityWebRequest = _unityWebRequest
                        }
                    );

                    // Merge any user-provided custom context data
                    if (customContextData != null)
                    {
                        CustomDataHelper.SetCustomData(ref customDataDic, customContextData);
                    }

                    // Attempt to infer .zip status from the Content-Type header
                    var contentType = unityWebRequest.GetResponseHeader("Content-Type");
                    if (contentType != null && isZipFile == null)
                    {
                        isZipFile = contentType.Contains("application/zip")
                            || contentType.Contains("application/x-zip-compressed")
                            || contentType.Contains("multipart/x-zip");
                    }

                    // Infer file extension if not given
                    if (!isZipFile.GetValueOrDefault() && string.IsNullOrWhiteSpace(fileExtension))
                    {
                        fileExtension = FileUtils.GetFileExtension(unityWebRequest.url);
                    }

                    // Load from .zip or from a regular file stream
                    if (isZipFile.GetValueOrDefault())
                    {
                        _assetLoaderContext = AssetLoaderZip.LoadModelFromZipStream(
                            memoryStream,
                            onLoad,
                            onMaterialsLoad,
                            delegate (AssetLoaderContext assetLoaderContext, float progressValue)
                            {
                                onProgress?.Invoke(assetLoaderContext, 0.5f + progressValue * 0.5f);
                            },
                            onError,
                            wrapperGameObject,
                            assetLoaderOptions,
                            customDataDic,
                            fileExtension
                        );
                    }
                    else
                    {
                        _assetLoaderContext = AssetLoader.LoadModelFromStream(
                            memoryStream,
                            null,
                            fileExtension,
                            onLoad,
                            onMaterialsLoad,
                            delegate (AssetLoaderContext assetLoaderContext, float progressValue)
                            {
                                onProgress?.Invoke(assetLoaderContext, 0.5f + progressValue * 0.5f);
                            },
                            onError,
                            wrapperGameObject,
                            assetLoaderOptions,
                            customDataDic
                        );
                    }
                }
                else
                {
                    // Construct an exception with the error message and response code
                    var exception = new Exception(
                        $"UnityWebRequest error: {unityWebRequest.error}, code: {unityWebRequest.responseCode}"
                    );
                    throw exception;
                }
            }
            catch (Exception exception)
            {
                // If an error callback is provided, wrap or forward the exception
                if (onError != null)
                {
                    var contextualizedError = exception as IContextualizedError;
                    onError(contextualizedError ?? new ContextualizedError<AssetLoaderContext>(exception, null));
                }
                else
                {
                    // Otherwise, rethrow the exception
                    throw;
                }
            }

            // Destroy the temporary GameObject once loading is complete or has failed
            Destroy(gameObject);
        }

        /// <summary>
        /// Called once per frame by Unity. Updates the current download progress in real time. 
        /// The progress value here represents the download portion (0–50% of overall loading),
        /// while the remaining 50% is tracked as the model is parsed and processed by TriLib.
        /// </summary>
        private void Update()
        {
            _onProgress?.Invoke(_assetLoaderContext, _unityWebRequest.downloadProgress * 0.5F);
        }
    }
}
