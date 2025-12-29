using System.Collections.Generic;
using TriLibCore.General;
using TriLibCore.Interfaces;
using UnityEngine;

namespace TriLibCore.Mappers
{
    /// <summary>
    /// Provides a lip sync mapping strategy by searching for viseme matches in blend‐shape key names.
    /// This mapper searches through candidate names specified in <see cref="VisemeCandidates"/> and compares them
    /// with the names of blend‐shape keys found in the model's geometry. If a match is found (using the configured
    /// string comparison options), the corresponding blend‐shape key index is returned.
    /// </summary>
    [CreateAssetMenu(menuName = "TriLib/Mappers/LypSync/By Name Lip Sync Mapper", fileName = "ByNameLipSyncMapper")]
    public class ByNameLipSyncMapper : LipSyncMapper
    {
        /// <summary>
        /// Defines the string comparison mode used when matching blend‐shape key names against viseme candidate names.
        /// </summary>
        /// <remarks>
        /// The "left" value in the comparison is the name of the blend‐shape key and the "right" value is the candidate name.
        /// </remarks>
        [Header("Left = Blend-Shape Key Name, Right = Viseme Name")]
        public StringComparisonMode StringComparisonMode;

        /// <summary>
        /// Indicates whether string comparisons for mapping visemes should be case insensitive.
        /// </summary>
        public bool CaseInsensitive = true;

        /// <summary>
        /// A list of viseme candidates. Each <see cref="VisemeCandidate"/> defines a viseme and a collection of 
        /// acceptable blend‐shape names that can be used to represent that viseme.
        /// </summary>
        public List<VisemeCandidate> VisemeCandidates;

        /// <inheritdoc />
        /// <remarks>
        /// This method iterates over each <see cref="VisemeCandidate"/> that matches the provided <paramref name="viseme"/>.
        /// For each candidate name, it compares the candidate string with the name of each blend‐shape key in the 
        /// <paramref name="geometryGroup"/> using <see cref="Utils.StringComparer.Matches"/> with the specified 
        /// <see cref="StringComparisonMode"/> and <see cref="CaseInsensitive"/> settings. If a match is found, the 
        /// method returns the index of the matching blend‐shape key; otherwise, it returns -1.
        /// </remarks>
        /// <param name="assetLoaderContext">
        /// The <see cref="AssetLoaderContext"/> containing the model loading data.
        /// </param>
        /// <param name="viseme">
        /// The viseme to map.
        /// </param>
        /// <param name="geometryGroup">
        /// The geometry group that contains the blend‐shape keys.
        /// </param>
        /// <returns>
        /// The index of the blend‐shape key corresponding to the viseme if a match is found; otherwise, -1.
        /// </returns>
        protected override int MapViseme(AssetLoaderContext assetLoaderContext, LipSyncViseme viseme, IGeometryGroup geometryGroup)
        {
            for (var i = 0; i < VisemeCandidates.Count; i++)
            {
                var visemeCandidate = VisemeCandidates[i];
                if (visemeCandidate.Viseme == viseme)
                {
                    for (var k = 0; k < visemeCandidate.CandidateNames.Count; k++)
                    {
                        var candidateName = visemeCandidate.CandidateNames[k];
                        for (var j = 0; j < geometryGroup.BlendShapeKeys.Count; j++)
                        {
                            var blendShapeGeometryBinding = geometryGroup.BlendShapeKeys[j];
                            if (Utils.StringComparer.Matches(StringComparisonMode, CaseInsensitive, blendShapeGeometryBinding.Name, candidateName))
                            {
                                return j;
                            }
                        }
                    }
                }
            }
            return -1;
        }
    }
}
