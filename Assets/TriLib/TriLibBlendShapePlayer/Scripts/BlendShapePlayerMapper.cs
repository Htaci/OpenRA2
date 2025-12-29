using UnityEngine;
using System;
using System.Collections.Generic;
using TriLibCore.Interfaces;
using TriLibCore.Mappers;

namespace TriLibCore.BlendShapePlayer
{
    /// <summary>
    /// A specialized <see cref="BlendShapeMapper"/> that creates and configures a 
    /// <see cref="BlendShapePlayer"/> component for the specified GameObject.
    /// The <see cref="BlendShapePlayerMapper"/> assigns blend shape keys and sets up animation curve mapping 
    /// for blend shapes based on their index.
    /// </summary>
    [CreateAssetMenu(menuName = "TriLib/Mappers/BlendShape/BlendShape Player Mapper", fileName = "BlendShapePlayerMapper")]
    public class BlendShapePlayerMapper : BlendShapeMapper
    {
        /// <inheritdoc />
        /// <remarks>
        /// This method adds a <see cref="BlendShapePlayer"/> component to the provided <paramref name="meshGameObject"/>,
        /// and calls its <see cref="BlendShapePlayer.Setup(IGeometryGroup, List{IBlendShapeKey})"/> method to initialize it with
        /// the given <paramref name="geometryGroup"/> and <paramref name="blendShapeKeys"/>.
        /// </remarks>
        public override void Setup(AssetLoaderContext assetLoaderContext, IGeometryGroup geometryGroup, GameObject meshGameObject, List<IBlendShapeKey> blendShapeKeys)
        {
            var blendShapePlayer = meshGameObject.AddComponent<BlendShapePlayer>();
            blendShapePlayer.Setup(geometryGroup, blendShapeKeys);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Maps a blend shape key index to an animation curve property name by delegating the lookup
        /// to <see cref="BlendShapePlayer.GetPropertyName(int)"/>.
        /// </remarks>
        public override string MapAnimationCurve(AssetLoaderContext assetLoaderContext, int blendShapeIndex)
        {
            return BlendShapePlayer.GetPropertyName(blendShapeIndex);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Returns the <see cref="Type"/> of the source component used for animation curve mapping, which is <see cref="BlendShapePlayer"/>.
        /// </remarks>
        public override Type AnimationCurveSourceType => typeof(BlendShapePlayer);
    }
}
