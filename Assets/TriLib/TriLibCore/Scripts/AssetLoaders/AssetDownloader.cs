using System;
using UnityEngine;
using UnityEngine.Networking;

namespace TriLibCore
{
    /// <summary>
    /// Provides functionality for downloading 3D model data from a specified URI and loading
    /// it into Unity. This class supports various HTTP methods (GET, POST, PUT, DELETE, HEAD) 
    /// and offers optional asynchronous behavior. The downloaded model can be placed within 
    /// a specified <see cref="GameObject"/> hierarchy, and can handle .zip archives if requested.
    /// </summary>
    public class AssetDownloader
    {
        /// <summary>
        /// Represents the HTTP request methods supported by <see cref="CreateWebRequest"/>.
        /// </summary>
        public enum HttpRequestMethod
        {
            /// <summary>
            /// The HTTP GET method, used to request data from a specified resource.
            /// </summary>
            Get,

            /// <summary>
            /// The HTTP POST method, used to submit data to be processed to a specified resource.
            /// </summary>
            Post,

            /// <summary>
            /// The HTTP PUT method, used to upload or replace a resource on the server.
            /// </summary>
            Put,

            /// <summary>
            /// The HTTP DELETE method, used to delete the specified resource.
            /// </summary>
            Delete,

            /// <summary>
            /// The HTTP HEAD method, used to request only the headers from the server for a resource.
            /// </summary>
            Head
        }

        /// <summary>
        /// Creates a new <see cref="UnityWebRequest"/> based on the specified parameters, allowing 
        /// you to configure the request method, data, and timeout. The returned request can be used 
        /// to download or upload data via HTTP.
        /// </summary>
        /// <param name="uri">The URI (URL) to which the request is sent.</param>
        /// <param name="httpRequestMethod">
        /// The HTTP method to use (e.g., <see cref="HttpRequestMethod.Get"/>, <see cref="HttpRequestMethod.Post"/>, etc.).
        /// Defaults to <see cref="HttpRequestMethod.Get"/>.
        /// </param>
        /// <param name="data">
        /// Optional data or parameters to include in the request. For example, in a GET request,
        /// this data will be appended as query parameters.
        /// </param>
        /// <param name="timeout">
        /// The request timeout in seconds. Defaults to 2000 seconds.
        /// </param>
        /// <returns>
        /// A configured <see cref="UnityWebRequest"/> instance with the specified method, data,
        /// and timeout settings.
        /// </returns>
        public static UnityWebRequest CreateWebRequest(
            string uri,
            HttpRequestMethod httpRequestMethod = HttpRequestMethod.Get,
            string data = null,
            int timeout = 2000)
        {
            UnityWebRequest unityWebRequest;
            switch (httpRequestMethod)
            {
                case HttpRequestMethod.Post:
#if UNITY_2022_2_OR_NEWER
                    unityWebRequest = UnityWebRequest.PostWwwForm(uri, data);
#else
                    unityWebRequest = UnityWebRequest.Post(uri, data);
#endif
                    break;
                case HttpRequestMethod.Put:
                    unityWebRequest = UnityWebRequest.Put(uri, data);
                    break;
                case HttpRequestMethod.Delete:
                    unityWebRequest = UnityWebRequest.Delete(
                        data != null ? $"{uri}?{data}" : uri
                    );
                    break;
                case HttpRequestMethod.Head:
                    unityWebRequest = UnityWebRequest.Head(
                        data != null ? $"{uri}?{data}" : uri
                    );
                    break;
                default:
                    unityWebRequest = UnityWebRequest.Get(
                        data != null ? $"{uri}?{data}" : uri
                    );
                    break;
            }
            unityWebRequest.timeout = timeout;
            return unityWebRequest;
        }

        /// <summary>
        /// Initiates an asynchronous download of 3D model data from the specified <see cref="UnityWebRequest"/>,
        /// then loads the model (optionally including .zip extraction). The method returns a 
        /// <see cref="Coroutine"/> which you can yield on or allow to run in the background. 
        /// Once the model is downloaded, TriLib’s <see cref="AssetLoader"/> will process the data 
        /// and create corresponding <see cref="GameObject"/>s, materials, and other resources.
        /// </summary>
        /// <param name="unityWebRequest">
        /// The configured <see cref="UnityWebRequest"/> to download the model. You can create this request
        /// using <see cref="CreateWebRequest"/> or provide a custom request.
        /// </param>
        /// <param name="onLoad">
        /// A callback invoked on the main Unity thread once the model data has been read, 
        /// but before materials are fully loaded. This is useful for performing actions 
        /// immediately after the base model structure is available.
        /// </param>
        /// <param name="onMaterialsLoad">
        /// A callback invoked on the main Unity thread once all resources (textures, shaders, etc.) 
        /// have been loaded. This indicates the final stage of model loading.
        /// </param>
        /// <param name="onProgress">
        /// A callback invoked whenever the loading progress updates, with a float parameter 
        /// indicating progress (0 to 1).
        /// </param>
        /// <param name="onError">
        /// A callback invoked on the main Unity thread if any error occurs during the download 
        /// or loading process.
        /// </param>
        /// <param name="wrapperGameObject">
        /// An optional parent <see cref="GameObject"/> under which the loaded model's <see cref="GameObject"/> 
        /// will be placed. If <c>null</c>, the model is created at the root level.
        /// </param>
        /// <param name="assetLoaderOptions">
        /// The <see cref="AssetLoaderOptions"/> controlling how the model is processed, 
        /// including material, animation, and texture loading behaviors.
        /// </param>
        /// <param name="customContextData">
        /// Optional custom data passed to the <see cref="AssetLoaderContext"/> for use in the loading pipeline.
        /// </param>
        /// <param name="fileExtension">
        /// If known, specify the model file extension (e.g., ".fbx"). 
        /// If the file is within a .zip, this should be the extension of the internal file.
        /// </param>
        /// <param name="isZipFile">
        /// Pass <c>true</c> if the file is a .zip, allowing TriLib to extract and process 
        /// the contained model. If <c>null</c>, TriLib will try to guess based on the <paramref name="fileExtension"/>.
        /// </param>
        /// <param name="haltTask">
        /// If <c>true</c>, the loading tasks are created but not started immediately. 
        /// Use this to chain multiple tasks or delay the loading process.
        /// </param>
        /// <returns>
        /// A <see cref="Coroutine"/> reference that represents the ongoing download and model loading 
        /// process. You can yield on it in a Unity coroutine or let it run asynchronously.
        /// </returns>
        public static Coroutine LoadModelFromUri(
            UnityWebRequest unityWebRequest,
            Action<AssetLoaderContext> onLoad,
            Action<AssetLoaderContext> onMaterialsLoad,
            Action<AssetLoaderContext, float> onProgress,
            Action<IContextualizedError> onError = null,
            GameObject wrapperGameObject = null,
            AssetLoaderOptions assetLoaderOptions = null,
            object customContextData = null,
            string fileExtension = null,
            bool? isZipFile = null,
            bool haltTask = false)
        {
            // Create a temporary GameObject with an attached AssetDownloaderBehaviour
            // to manage the coroutine lifecycle.
            var assetDownloader = new GameObject("Asset Downloader").AddComponent<AssetDownloaderBehaviour>();
            return assetDownloader.StartCoroutine(
                assetDownloader.DownloadAsset(
                    unityWebRequest,
                    onLoad,
                    onMaterialsLoad,
                    onProgress,
                    wrapperGameObject,
                    onError,
                    assetLoaderOptions,
                    customContextData,
                    fileExtension,
                    isZipFile
                )
            );
        }
    }
}
