using System;
using System.Collections;
using TriLibCore.General;
using TriLibCore.Gltf;
using TriLibCore.Gltf.Reader;
using TriLibCore.Utils;
using UnityEngine;

namespace TriLibCore.Mappers
{
    /// <summary>
    /// Converts TriLib virtual materials of glTF2 assets into Unity Standard materials,
    /// using shader properties and material presets appropriate for the currently active render pipeline.
    /// </summary>
    /// <remarks>
    /// This mapper supports both the specular-glossiness and metallic-roughness workflows.
    /// Based on the flag <see cref="UsingSpecularGlossiness"/>, it selects a material preset from HDRP, URP, or Standard
    /// resource directories. It forces the use of a shader variant collection and extracts metallic and smoothness properties.
    /// The mapper is considered compatible if the reader is a <see cref="GltfReader"/>.
    /// </remarks>
    [Serializable]
    [CreateAssetMenu(menuName = "TriLib/Mappers/Material/glTF2 Standard Material Mapper", fileName = "glTF2StandardMaterialMapper")]
    public class glTF2StandardMaterialMapper : StandardMaterialMapper
    {
        public override bool UseShaderVariantCollection => true;
        protected bool UsingSpecularGlossiness;

        public override Material MaterialPreset
        {
            get
            {
                if (UsingSpecularGlossiness)
                {

                    if (GraphicsSettingsUtils.IsUsingHDRPPipeline)
                    {
                        return Resources.Load<Material>("Materials/glTF2/HDRP/HDRPSpecularGLTF2");
                    }
                    if (GraphicsSettingsUtils.IsUsingUniversalPipeline)
                    {
                        return Resources.Load<Material>("Materials/glTF2/UniversalRP/UniversalRPSpecularGLTF2");
                    }
                    return Resources.Load<Material>("Materials/glTF2/Standard/StandardSpecularGLTF2");
                }
                else
                {
                    if (GraphicsSettingsUtils.IsUsingHDRPPipeline)
                    {
                        return Resources.Load<Material>("Materials/glTF2/HDRP/HDRPGLTF2");
                    }
                    if (GraphicsSettingsUtils.IsUsingUniversalPipeline)
                    {
                        return Resources.Load<Material>("Materials/glTF2/UniversalRP/UniversalRPGLTF2");
                    }
                    return Resources.Load<Material>("Materials/glTF2/Standard/StandardGLTF2");
                }
            }
        }

        public override Material LoadingMaterial => MaterialPreset;

        public override Material CutoutMaterialPreset => MaterialPreset;

        public override Material TransparentComposeMaterialPreset => MaterialPreset;

        public override Material TransparentMaterialPreset => MaterialPreset;

        public override Material MaterialPresetNoMetallicTexture => MaterialPreset;

        public override Material CutoutMaterialPresetNoMetallicTexture => MaterialPreset;

        public override Material TransparentMaterialPresetNoMetallicTexture => MaterialPreset;

        public override Material TransparentComposeMaterialPresetNoMetallicTexture => MaterialPreset;

        public override bool ExtractMetallicAndSmoothness => true;

        public override IEnumerable MapCoroutine(MaterialMapperContext materialMapperContext)
        {
            UsingSpecularGlossiness = materialMapperContext.Material is GltfMaterial gltfMaterial && gltfMaterial.UsingSpecularGlossinness;
            return base.MapCoroutine(materialMapperContext);
        }

        public override bool IsCompatible(MaterialMapperContext materialMapperContext)
        {
            return materialMapperContext != null && materialMapperContext.Context.Reader is GltfReader;
        }

        protected override IEnumerable ApplyGlossinessMapTexture(TextureLoadingContext textureLoadingContext)
        {
            if (textureLoadingContext.UnityTexture != null)
            {
                textureLoadingContext.Context.AddUsedTexture(textureLoadingContext.UnityTexture);
            }
            textureLoadingContext.MaterialMapperContext.VirtualMaterial.SetProperty("_SpecGlossMap", textureLoadingContext.UnityTexture, GenericMaterialProperty.MetallicMap);
            if (textureLoadingContext.UnityTexture != null)
            {
                textureLoadingContext.MaterialMapperContext.VirtualMaterial.EnableKeyword("_SPECGLOSSMAP");
            }
            else
            {
                textureLoadingContext.MaterialMapperContext.VirtualMaterial.DisableKeyword("_SPECGLOSSMAP");
            }
            yield break;
        }

        protected override IEnumerable CheckGlossinessValue(MaterialMapperContext materialMapperContext)
        {
            var value = 1f - materialMapperContext.Material.GetGenericFloatValueMultiplied(GenericMaterialProperty.GlossinessOrRoughness, materialMapperContext);
            materialMapperContext.VirtualMaterial.SetProperty("_Glossiness", value);
            materialMapperContext.VirtualMaterial.SetProperty("_GlossMapScale", value);
            yield break;
        }

        public override string GetGlossinessOrRoughnessName(MaterialMapperContext materialMapperContext)
        {
            return "_Glossiness";
        }

    }
}