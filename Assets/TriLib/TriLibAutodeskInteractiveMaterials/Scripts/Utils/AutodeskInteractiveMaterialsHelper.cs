using TriLibCore.Mappers;
using UnityEngine;

namespace TriLibCore.Utils
{
    /// <summary>
    /// Provides helper methods to configure an <see cref="AssetLoaderOptions"/> instance 
    /// with the <see cref="AutodeskInteractiveStandardMaterialMapper"/>, ensuring correct 
    /// interpretation and rendering of Autodesk Interactive materials.
    /// </summary>
    [CreateAssetMenu(
        menuName = "TriLib/MaterialsHelper/Autodesk Interactive Materials Helper",
        fileName = "AutodeskInteractiveMaterialsHelper")]
    public class AutodeskInteractiveMaterialsHelper : MaterialsHelper
    {
        /// <summary>
        /// A static convenience method to create a temporary instance of 
        /// <see cref="AutodeskInteractiveMaterialsHelper"/> and call 
        /// <see cref="Setup(ref AssetLoaderOptions)"/> on the provided 
        /// <paramref name="assetLoaderOptions"/>.
        /// </summary>
        /// <param name="assetLoaderOptions">
        /// A reference to an existing <see cref="AssetLoaderOptions"/>. If <c>null</c>, 
        /// a default loader options object is created before applying the Autodesk Interactive settings.
        /// </param>
        public static void SetupStatic(ref AssetLoaderOptions assetLoaderOptions)
        {
            CreateInstance<AutodeskInteractiveMaterialsHelper>().Setup(ref assetLoaderOptions);
        }

        /// <summary>
        /// Configures the specified <paramref name="assetLoaderOptions"/> to use 
        /// the <see cref="AutodeskInteractiveStandardMaterialMapper"/>, adding it to the list 
        /// of <see cref="MaterialMapper"/> instances. This method also adjusts certain 
        /// properties of <see cref="AssetLoaderOptions"/> to better handle 
        /// Autodesk Interactive material data (e.g., allowing displacement textures).
        /// </summary>
        /// <param name="assetLoaderOptions">
        /// A reference to the <see cref="AssetLoaderOptions"/> object being configured.
        /// If <c>null</c>, a default instance is created automatically.
        /// </param>
        public override void Setup(ref AssetLoaderOptions assetLoaderOptions)
        {
            var autodeskInteractiveMaterialMapper =
                ScriptableObject.CreateInstance<AutodeskInteractiveStandardMaterialMapper>();

            if (autodeskInteractiveMaterialMapper != null)
            {
                // Ensure a valid AssetLoaderOptions instance exists
                if (assetLoaderOptions == null)
                {
                    assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
                }

                // Append the Autodesk interactive material mapper
                if (assetLoaderOptions.MaterialMappers == null)
                {
                    assetLoaderOptions.MaterialMappers =
                        new MaterialMapper[] { autodeskInteractiveMaterialMapper };
                }
                else
                {
                    ArrayUtils.Add(ref assetLoaderOptions.MaterialMappers, autodeskInteractiveMaterialMapper);
                }

                // Configure additional asset loader settings for Autodesk Interactive usage
                assetLoaderOptions.CreateMaterialsForAllModels = true;
                assetLoaderOptions.SetUnusedTexturePropertiesToNull = false;
                assetLoaderOptions.ConvertMaterialTextures = false;
                assetLoaderOptions.LoadDisplacementTextures = true;

                // Set the mapper’s checking order to prioritize or arrange it among other mappers
                autodeskInteractiveMaterialMapper.CheckingOrder = 1;
            }
        }
    }
}
