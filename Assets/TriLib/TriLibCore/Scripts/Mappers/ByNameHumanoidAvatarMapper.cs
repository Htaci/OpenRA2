using System.Collections.Generic;
using TriLibCore.General;
using UnityEngine;
using HumanLimit = TriLibCore.General.HumanLimit;
using TriLibCore.Utils;

namespace TriLibCore.Mappers
{
    /// <summary>
    /// Implements a humanoid avatar mapping strategy by matching the names of loaded GameObjects with 
    /// candidate bone names specified in a bone mapping list. For each human bone defined in the mapping, the first 
    /// matching GameObject found in the model hierarchy is used. If a required bone is not found, a warning is issued.
    /// </summary>
    [CreateAssetMenu(menuName = "TriLib/Mappers/Humanoid/By Name Humanoid Avatar Mapper", fileName = "ByNameHumanoidAvatarMapper")]
    public class ByNameHumanoidAvatarMapper : HumanoidAvatarMapper
    {
        /// <summary>
        /// Specifies the string comparison mode to use when matching loaded GameObject names 
        /// against candidate names defined in the bone mapping.
        /// </summary>
        /// <remarks>
        /// The "left" value is the name of the loaded GameObject and the "right" value is one of the candidate names.
        /// </remarks>
        [Header("Left = Loaded GameObjects Names, Right = Names in BonesMapping.BoneNames")]
        public StringComparisonMode stringComparisonMode;

        /// <summary>
        /// Indicates whether string comparisons are performed in a case-insensitive manner.
        /// </summary>
        public bool CaseInsensitive = true;

        /// <summary>
        /// A list of bone mappings that define how human bones (e.g. <c>Hips</c>, <c>Spine</c>) map to possible 
        /// GameObject names in the imported model.
        /// </summary>
        public List<BoneMapping> BonesMapping;

        /// <summary>
        /// Recursively searches the transform hierarchy starting at <paramref name="transform"/> for children whose names match 
        /// the specified <paramref name="targetName"/> (using the given string comparison settings). 
        /// All matching transforms are added to the <paramref name="matches"/> list.
        /// </summary>
        /// <param name="transform">The root transform from which to begin the search.</param>
        /// <param name="targetName">The candidate bone name to match.</param>
        /// <param name="stringComparisonMode">The string comparison mode to use.</param>
        /// <param name="caseInsensitive">If set to <c>true</c>, comparison will be case insensitive.</param>
        /// <param name="matches">A list to which any matching transforms are added.</param>
        private static void FindDeepChildList(Transform transform, string targetName, StringComparisonMode stringComparisonMode, bool caseInsensitive, List<Transform> matches)
        {
            if (StringComparer.Matches(stringComparisonMode, caseInsensitive, transform.name, targetName))
            {
                matches.Add(transform);
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                FindDeepChildList(child, targetName, stringComparisonMode, caseInsensitive, matches);
            }
        }

        /// <inheritdoc />
        public override Dictionary<BoneMapping, Transform> Map(AssetLoaderContext assetLoaderContext)
        {
            var mapping = new Dictionary<BoneMapping, Transform>();
            var matches = new List<Transform>();
            for (var i = 0; i < BonesMapping.Count; i++)
            {
                var boneMapping = BonesMapping[i];
                if (boneMapping.BoneNames != null)
                {
                    bool found = false;
                    for (var j = 0; j < boneMapping.BoneNames.Length; j++)
                    {
                        matches.Clear();
                        var boneName = boneMapping.BoneNames[j];
                        FindDeepChildList(assetLoaderContext.RootGameObject.transform, boneName, stringComparisonMode, CaseInsensitive, matches);
                        foreach (var transform in matches)
                        {
                            if (transform == null)
                            {
                                continue;
                            }
                            var model = assetLoaderContext.Models[transform.gameObject];
                            if (!model.IsBone)
                            {
                                continue;
                            }
                            mapping.Add(boneMapping, transform);
                            found = true;
                            break;
                        }
                        if (found)
                        {
                            break;
                        }
                    }
                    // If a required bone is not found, log a warning and return an empty mapping.
                    if (!found && !IsBoneOptional(boneMapping.HumanBone))
                    {
                        if (assetLoaderContext.Options.ShowLoadingWarnings)
                        {
                            Debug.LogWarning($"Could not find bone '{boneMapping.HumanBone}'");
                        }
                        mapping.Clear();
                        return mapping;
                    }
                }
            }
            return mapping;
        }

        /// <summary>
        /// Determines whether a particular human bone is optional.
        /// </summary>
        /// <param name="humanBodyBones">The human bone type to check.</param>
        /// <returns>
        /// <c>true</c> if the specified bone is not required; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsBoneOptional(HumanBodyBones humanBodyBones)
        {
            return !HumanTrait.RequiredBone((int)humanBodyBones);
        }

        /// <summary>
        /// Adds a new bone mapping to the current list of mappings. This method allows dynamically 
        /// adding an entry that associates a specific human bone with one or more candidate transform names 
        /// and an optional human limit.
        /// </summary>
        /// <param name="humanBodyBones">
        /// The human bone (as defined in <see cref="HumanBodyBones"/>) to map.
        /// </param>
        /// <param name="humanLimit">
        /// The constraints or limits associated with this bone.
        /// </param>
        /// <param name="boneNames">
        /// The candidate transform names to search for in the model hierarchy.
        /// </param>
        public void AddMapping(HumanBodyBones humanBodyBones, HumanLimit humanLimit, params string[] boneNames)
        {
            if (BonesMapping == null)
            {
                BonesMapping = new List<BoneMapping>();
            }
            BonesMapping.Add(new BoneMapping(humanBodyBones, humanLimit, boneNames));
        }
    }
}
