using UnityEngine.Networking;

namespace TriLibCore
{
    /// <summary>
    /// Holds additional context data related to URI-based (network) model loading. 
    /// Instances of this class are stored in an <see cref="AssetLoaderContext.CustomData"/> 
    /// object, allowing developers to track the originating <see cref="UnityWebRequest"/> 
    /// and related networking state.
    /// </summary>
    public class UriLoadCustomContextData
    {
        /// <summary>
        /// The <see cref="UnityWebRequest"/> instance used to download model data from a remote URI.
        /// This allows retrieval of response headers, status codes, or other network-related information 
        /// after the model has been loaded.
        /// </summary>
        public UnityWebRequest UnityWebRequest;
    }
}
