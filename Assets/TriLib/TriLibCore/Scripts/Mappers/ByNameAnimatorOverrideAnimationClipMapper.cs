using System.Collections.Generic;
using TriLibCore.General;
using TriLibCore.Utils;
using UnityEngine;

namespace TriLibCore.Mappers
{
    /// <summary>
    /// Implements an <see cref="AnimatorOverrideAnimationClipMapper"/> that maps animation clips 
    /// by matching their names to those defined in an Animator Override Controller.
    /// This mapper uses a configurable string comparison mode and a case-insensitivity option 
    /// to determine if a loaded animation clip matches a key in the override controller.
    /// </summary>
    [CreateAssetMenu(menuName = "TriLib/Mappers/Animation Clip/By Name Animator Override Animation Clip Mapper", fileName = "ByNameAnimatorOverrideAnimationClipMapper")]
    public class ByNameAnimatorOverrideAnimationClipMapper : AnimatorOverrideAnimationClipMapper
    {
        /// <summary>
        /// Specifies the string comparison mode for matching animation clip names 
        /// against the keys in the Animator Override Controller.
        /// </summary>
        /// <remarks>
        /// This setting is used to compare the “left” side of the mapping (the names defined in the 
        /// Animator Override Controller) to the “right” side (the names of the loaded animation clips).
        /// </remarks>
        [Header("Left = Animator Override Clip Names, Right = Loaded Clip Names")]
        public StringComparisonMode StringComparisonMode;

        /// <summary>
        /// Gets or sets a value indicating whether the string comparison should be case insensitive.
        /// </summary>
        public bool CaseInsensitive = true;

        /// <inheritdoc />
        /// <remarks>
        /// This method iterates through each source animation clip and retrieves the list of current 
        /// overrides from the <see cref="AnimatorOverrideController"/>. For each override, if the name of 
        /// the key and the current animation clip match (based on the configured string comparison settings), 
        /// the override value is updated with the loaded animation clip.
        /// After processing all clips, the updated overrides are applied to the Animator Override Controller,
        /// and the base class mapping logic is executed.
        /// </remarks>
        public override AnimationClip[] MapArray(AssetLoaderContext assetLoaderContext, AnimationClip[] sourceAnimationClips)
        {
            if (AnimatorOverrideController != null)
            {
                for (var i = 0; i < sourceAnimationClips.Length; i++)
                {
                    var animationClip = sourceAnimationClips[i];
                    var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(AnimatorOverrideController.overridesCount);
                    AnimatorOverrideController.GetOverrides(overrides);
                    for (var j = 0; j < overrides.Count; j++)
                    {
                        var kvp = overrides[j];
                        var keyName = kvp.Key.name;
                        var clipName = animationClip.name;
                        if (StringComparer.Matches(StringComparisonMode, CaseInsensitive, keyName, clipName))
                        {
                            overrides[j] = new KeyValuePair<AnimationClip, AnimationClip>(kvp.Key, animationClip);
                        }
                    }
                    AnimatorOverrideController.ApplyOverrides(overrides);
                }
            }
            return base.MapArray(assetLoaderContext, sourceAnimationClips);
        }
    }
}
