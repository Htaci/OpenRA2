using UnityEngine;

namespace TriLibCore.Mappers
{
    /// <summary>
    /// Provides a mechanism for applying an <see cref="AnimatorOverrideController"/> to the animator 
    /// of a loaded model while processing animation clips. This mapper is designed to work with 
    /// Animator Override Controllers, allowing you to override default animation clips with custom ones.
    /// </summary>
    public class AnimatorOverrideAnimationClipMapper : AnimationClipMapper
    {
        /// <summary>
        /// The Animator Override Controller to assign to the model’s <see cref="Animator"/>.
        /// If not set, the mapper will not override the animator controller.
        /// </summary>
        public AnimatorOverrideController AnimatorOverrideController;

        /// <inheritdoc />
        /// <remarks>
        /// This method retrieves the <see cref="Animator"/> component from the root GameObject in the 
        /// Asset Loader Context and assigns the specified <see cref="AnimatorOverrideController"/> to it.
        /// If either the animator or the override controller is missing, a warning is issued (if enabled in the options)
        /// and the original animation clips are returned unmodified.
        /// </remarks>
        public override AnimationClip[] MapArray(AssetLoaderContext assetLoaderContext, AnimationClip[] sourceAnimationClips)
        {
            var animator = assetLoaderContext.RootGameObject.GetComponent<Animator>();
            if (animator == null || AnimatorOverrideController == null)
            {
                if (assetLoaderContext.Options.ShowLoadingWarnings)
                {
                    Debug.LogWarning("Tried to execute an AnimatorOverrideController Mapper on a GameObject without an Animator or without setting an AnimatorOverrideController.");
                }
                return sourceAnimationClips;
            }

            animator.runtimeAnimatorController = AnimatorOverrideController;
            return sourceAnimationClips;
        }
    }
}
