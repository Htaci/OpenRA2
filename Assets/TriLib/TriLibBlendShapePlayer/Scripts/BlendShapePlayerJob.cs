using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace TriLibCore.BlendShapePlayer
{
    /// <summary>
    /// Represents a Job that plays BlendShapeKeys efficiently.
    /// </summary>
    public struct BlendShapePlayerJob : IJobParallelFor
    {
        [ReadOnly]
        private NativeArray<Vector3> _blendShapeVertices;
        [ReadOnly]
        private NativeArray<Vector3> _originalVertices;
        [ReadOnly]
        private NativeArray<float> _weights;
        [ReadOnly]
        private NativeArray<float> _frameWeights;
        private NativeArray<Vector3> _modifiedVertices;

        /// <summary>
        /// Creates the BlendShapePlayer job.
        /// </summary>
        /// <param name="blendShapeVertices">The list of all BlendShapeKey vertices.</param>
        /// <param name="originalVertices">The list of the source Mesh original vertices.</param>
        /// <param name="modifiedVertices">The list of the source Mesh modified vertices.</param>
        /// <param name="weights">The list of BlendShapeKey current weights.</param>
        /// <param name="frameWeights">The list of BlendShapeKey maximum weights.</param>
        public BlendShapePlayerJob(NativeArray<Vector3> blendShapeVertices, NativeArray<Vector3> originalVertices, NativeArray<Vector3> modifiedVertices, NativeArray<float> weights, NativeArray<float> frameWeights)
        {
            _blendShapeVertices = blendShapeVertices;
            _originalVertices = originalVertices;
            _modifiedVertices = modifiedVertices;
            _weights = weights;
            _frameWeights = frameWeights;
        }

        /// <summary>
        /// Processes the BlendShapeKeys and applies the results to the modified vertices list.
        /// </summary>
        /// <param name="index"></param>
        public void Execute(int index)
        {
            _modifiedVertices[index] = _originalVertices[index];
            for (var weightIndex = 0; weightIndex < _weights.Length; weightIndex++)
            {
                var weight = _weights[weightIndex];
                if (weight == 0f)
                {
                    continue;
                }
                var frameWeight = _frameWeights[weightIndex];
                weight = frameWeight == 0f ? 0f : weight / frameWeight;
                _modifiedVertices[index] += _blendShapeVertices[weightIndex * _originalVertices.Length + index] * weight;
            }
        }
    }
}