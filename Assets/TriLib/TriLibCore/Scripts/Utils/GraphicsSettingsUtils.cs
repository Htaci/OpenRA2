using UnityEngine.Rendering;

namespace TriLibCore.Utils
{
    /// <summary>
    /// Represents a series of graphic settings utility methods.
    /// </summary>
    public static class GraphicsSettingsUtils
    {
        /// <summary>Returns <c>true</c> if the project is using the Standard Rendering Pipeline.</summary>
        public static bool IsUsingStandardPipeline => RenderPipelineUtils.IsUsingStandardPipeline;

        /// <summary>Returns <c>true</c> if the project is using the Universal Rendering Pipeline.</summary>
        public static bool IsUsingUniversalPipeline => RenderPipelineUtils.IsUsingUniversalPipeline;

        /// <summary>Returns <c>true</c> if the project is using the HDRP Rendering Pipeline.</summary>
        public static bool IsUsingHDRPPipeline => RenderPipelineUtils.IsUsingHDRPPipeline;
    }
}
