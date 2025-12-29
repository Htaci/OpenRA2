using TriLibCore.Mappers;
using TriLibCore.Utils;
using UnityEditor;
using UnityEngine;

namespace TriLibCore.Editor
{
    /// <summary>
    /// Represents a series of Material Mapper utility methods.
    /// </summary>
    [InitializeOnLoad]
    public static class CheckMappers
    {
        [MenuItem("Tools/TriLib/Select Material Mappers based on Rendering Pipeline")]
        private static void AutoSelect()
        {
            string materialMapperName;
            for (var i = 0; i < MaterialMapper.RegisteredMappers.Count; i++)
            {
                materialMapperName = MaterialMapper.RegisteredMappers[i];
                TriLibSettings.SetBool(materialMapperName, false);
            }
            materialMapperName = AssetLoader.GetCompatibleMaterialMapperName();
            SelectMapper(materialMapperName);
        }

        /// <summary>
        /// Enables a compatible Material Mapper if none is found.
        /// </summary>
        public static void EnableCompatibleMaterialMapperIfNeeded()
        {
            string materialMapperName;
            for (var i = 0; i < MaterialMapper.RegisteredMappers.Count; i++)
            {
                materialMapperName = MaterialMapper.RegisteredMappers[i];
                if (TriLibSettings.GetBool(materialMapperName, false))
                {
                    return;
                }
            }
            EnableCompatibleMaterialMapper();
        }

        /// <summary>
        /// Tries to find the best Material Mapper depending on the Rendering Pipeline.
        /// </summary>
        public static void EnableCompatibleMaterialMapper()
        {
            var usingMaterialMapper = false;
            for (var i = 0; i < MaterialMapper.RegisteredMappers.Count; i++)
            {
                var materialMapperName = MaterialMapper.RegisteredMappers[i];
                if (TriLibSettings.GetBool(materialMapperName))
                {
                    usingMaterialMapper = true;
                    break;
                }
            }
            if (!usingMaterialMapper)
            {
                var materialMapperName = AssetLoader.GetCompatibleMaterialMapperName();
                SelectMapper(materialMapperName);
            }
        }

        static CheckMappers()
        {
            EnableCompatibleMaterialMapperIfNeeded();
        }

        public static void SelectMapper(string materialMapper)
        {
            Debug.Log($"TriLib is configured to use the '{materialMapper}' Material Mapper. If you want to use different Material Mappers, you can change this setting on the 'Edit->Project Settings->TriLib' menu.");
            TriLibSettings.SetBool(materialMapper, true);
        }
    }
}