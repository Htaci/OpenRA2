using System.Collections.Generic;
using TriLibCore.Extensions;
using TriLibCore.Interfaces;
using UnityEngine;

namespace TriLibCore.Mappers
{
    /// <summary>
    /// Implements a root bone selection strategy by scanning a list of bones 
    /// (Transform components) and choosing the one with the greatest number of child transforms.
    /// This mapper is useful when the asset hierarchy does not provide a clear root bone 
    /// from naming conventions alone.
    /// </summary>
    [CreateAssetMenu(menuName = "TriLib/Mappers/Root Bone/By Bones Root Bone Mapper", fileName = "ByBonesRootBoneMapper")]
    public class ByBonesRootBoneMapper : RootBoneMapper
    {
        public override Transform Map(AssetLoaderContext assetLoaderContext, IList<Transform> bones)
        {
            Transform bestBone = null;
            var bestChildrenCount = 0;
            for (var i = 0; i < bones.Count; i++)
            {
                var bone = bones[i];
                var childrenCount = bone.CountChild();
                if (childrenCount >= bestChildrenCount)
                {
                    bestChildrenCount = childrenCount;
                    bestBone = bone;
                }
            }
            return bestBone;
        }
    }
}
