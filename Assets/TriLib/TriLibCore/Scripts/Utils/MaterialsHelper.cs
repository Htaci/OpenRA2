using UnityEngine;

namespace TriLibCore.Utils
{
    /// <summary>
    /// Provides helper methods for configuring <see cref="AssetLoaderOptions"/> to use different Material Mappers.
    /// </summary>
    public abstract class MaterialsHelper : ScriptableObject
    {
        /// <summary>
        /// Configures the specified <paramref name="assetLoaderOptions"/>.
        /// </summary>
        /// <param name="assetLoaderOptions">
        /// A reference to the <see cref="AssetLoaderOptions"/> object being configured.
        /// </param>
        public abstract void Setup(ref AssetLoaderOptions assetLoaderOptions);
    }
}