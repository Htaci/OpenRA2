using TriLibCore.Mappers;
using TriLibCore.Samples;
using UnityEngine;

namespace TriLibCore.Utils
{
    /// <summary>
    /// Provides helper methods for configuring <see cref="AssetLoaderOptions"/> to use the
    /// <see cref="glTF2StandardMaterialMapper"/>, ensuring that glTF 2.0 materials are applied 
    /// correctly during model loading.
    /// </summary>
    [CreateAssetMenu(menuName = "TriLib/MaterialsHelper/Gltf Materials Helper", fileName = "GltfMaterialsHelper")]
    public class GltfMaterialsHelper : MaterialsHelper
    {
        /// <summary>
        /// A static convenience method that creates an instance of <see cref="GltfMaterialsHelper"/>
        /// and invokes <see cref="Setup(ref AssetLoaderOptions)"/> to apply the <see cref="glTF2StandardMaterialMapper"/>
        /// and related options to <paramref name="assetLoaderOptions"/>.
        /// </summary>
        /// <param name="assetLoaderOptions">
        /// A reference to an existing <see cref="AssetLoaderOptions"/> object. If <c>null</c>, 
        /// this method generates a default set of loader options before configuring 
        /// glTF-specific materials.
        /// </param>
        public static void SetupStatic(ref AssetLoaderOptions assetLoaderOptions)
        {
            CreateInstance<GltfMaterialsHelper>().Setup(ref assetLoaderOptions);
        }

        /// <summary>
        /// Configures <paramref name="assetLoaderOptions"/> to use the <see cref="glTF2StandardMaterialMapper"/>,
        /// ensuring that glTF materials (such as PBR data) are translated correctly into Unity’s material system.
        /// This method also adjusts certain <see cref="AssetLoaderOptions"/> fields for optimal glTF handling.
        /// </summary>
        /// <param name="assetLoaderOptions">
        /// A reference to an <see cref="AssetLoaderOptions"/> object to configure. If it is <c>null</c>, 
        /// the method creates a default set of loader options.
        /// </param>
        public override void Setup(ref AssetLoaderOptions assetLoaderOptions)
        {
            var glTF2MaterialMapper = ScriptableObject.CreateInstance<glTF2StandardMaterialMapper>();
            if (glTF2MaterialMapper != null)
            {
                if (assetLoaderOptions == null)
                {
                    assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
                }
                if (assetLoaderOptions.MaterialMappers == null)
                {
                    assetLoaderOptions.MaterialMappers = new MaterialMapper[] { glTF2MaterialMapper };
                }
                else
                {
                    ArrayUtils.Add(ref assetLoaderOptions.MaterialMappers, glTF2MaterialMapper);
                }

                // Adjust relevant options for glTF usage
                assetLoaderOptions.CreateMaterialsForAllModels = true;
                assetLoaderOptions.SetUnusedTexturePropertiesToNull = false;
                assetLoaderOptions.ConvertMaterialTextures = false;

                // Assign a checking order to prioritize this mapper among others
                glTF2MaterialMapper.CheckingOrder = 2;
            }
        }
    }
}
