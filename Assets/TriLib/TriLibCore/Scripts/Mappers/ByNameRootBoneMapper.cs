using System.Collections.Generic;
using TriLibCore.General;
using TriLibCore.Utils;
using UnityEngine;

namespace TriLibCore.Mappers
{
    /// <summary>
    /// Implements a root bone selection strategy by searching for a matching bone name 
    /// within the loaded model’s hierarchy. This mapper iterates over a predefined list of 
    /// candidate root bone names (e.g., "Hips", "Bip01", "Pelvis") and returns the first match 
    /// found based on the configured string comparison settings.
    /// </summary>
    [CreateAssetMenu(menuName = "TriLib/Mappers/Root Bone/By Name Root Bone Mapper", fileName = "ByNameRootBoneMapper")]
    public class ByNameRootBoneMapper : RootBoneMapper
    {
        /// <summary>
        /// Specifies the string comparison mode for matching loaded GameObject names 
        /// against the candidate root bone names.
        /// </summary>
        /// <remarks>
        /// The "left" value is the name of the loaded GameObject and the "right" value is one of 
        /// the candidate names specified in <see cref="RootBoneNames"/>.
        /// </remarks>
        [Header("Left = Loaded GameObjects Names, Right = Names in RootBoneNames")]
        public StringComparisonMode StringComparisonMode;

        /// <summary>
        /// Indicates whether the string comparisons for matching root bone names should be case insensitive.
        /// </summary>
        public bool CaseInsensitive = true;

        /// <summary>
        /// A list of candidate root bone names to search for in the loaded model. 
        /// Examples might include "Hips", "Bip01", and "Pelvis".
        /// </summary>
        public string[] RootBoneNames = { "Hips", "Bip01", "Pelvis" };

        /// <inheritdoc />
        public override Transform Map(AssetLoaderContext assetLoaderContext, IList<Transform> bones)
        {
            if (RootBoneNames != null)
            {
                for (var i = 0; i < RootBoneNames.Length; i++)
                {
                    var rootBoneName = RootBoneNames[i];
                    var found = FindDeepChild(bones, rootBoneName);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            return base.Map(assetLoaderContext, bones);
        }

        /// <summary>
        /// Searches through the given list of transforms for a GameObject whose name matches the provided target name.
        /// </summary>
        /// <param name="transforms">
        /// A list of candidate transforms that may represent bones in the model.
        /// </param>
        /// <param name="right">
        /// The target name to search for.
        /// </param>
        /// <returns>
        /// The first <see cref="Transform"/> whose name matches the target (using the configured string comparison),
        /// or <c>null</c> if no match is found.
        /// </returns>
        private Transform FindDeepChild(IList<Transform> transforms, string right)
        {
            for (var i = 0; i < transforms.Count; i++)
            {
                var transform = transforms[i];
                if (StringComparer.Matches(StringComparisonMode, CaseInsensitive, transform.name, right))
                {
                    return transform;
                }
            }
            return null;
        }
    }
}
