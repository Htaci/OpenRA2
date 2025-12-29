using TriLibCore.General;
using TriLibCore.Playables;
using UnityEngine;

namespace TriLibCore.Mappers
{
    /// <summary>
    /// Implements an <see cref="AnimationClipMapper"/> that creates a <see cref="SimpleAnimationPlayer"/> 
    /// for playing animation clips by index or name. When the animation type is set to Generic or Humanoid,
    /// and at least one animation clip is available, this mapper adds a <see cref="SimpleAnimationPlayer"/> to 
    /// the model's root GameObject and assigns the animation clips to it.
    /// </summary>
    [CreateAssetMenu(menuName = "TriLib/Mappers/Animation Clip/Simple Animation Player Animation Clip Mapper", fileName = "SimpleAnimationPlayerAnimationClipMapper")]
    public class SimpleAnimationPlayerAnimationClipMapper : AnimationClipMapper
    {
        /// <inheritdoc />
        public override AnimationClip[] MapArray(AssetLoaderContext assetLoaderContext, AnimationClip[] sourceAnimationClips)
        {
            if ((assetLoaderContext.Options.AnimationType == AnimationType.Generic ||
                 assetLoaderContext.Options.AnimationType == AnimationType.Humanoid) && sourceAnimationClips.Length > 0)
            {
                var simpleAnimationPlayer = assetLoaderContext.RootGameObject.AddComponent<SimpleAnimationPlayer>();
                simpleAnimationPlayer.AnimationClips = sourceAnimationClips;
                simpleAnimationPlayer.enabled = false;
            }
            return sourceAnimationClips;
        }
    }
}
