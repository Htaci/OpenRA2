using System.IO;
using TriLibCore.Utils;
using UnityEngine;

namespace TriLibCore.Mappers
{
    /// <summary>
    /// Provides a texture mapping strategy which attempts to locate an external texture file 
    /// whose name matches the filename of the source model. The search is performed in the directory
    /// where the model file is located.
    /// </summary>
    [CreateAssetMenu(menuName = "TriLib/Mappers/Texture/Per Filename Texture Mapper")]
    public class PerFilenameTextureMapper : TextureMapper
    {
        /// <summary>
        /// Attempts to locate and open a data stream for a texture by matching the model file's short filename 
        /// with the short filenames of files found in the same directory.
        /// </summary>
        /// <param name="textureLoadingContext">
        /// The <see cref="TextureLoadingContext"/> containing the TriLib texture information, including the 
        /// model filename (via <see cref="AssetLoaderContext.Filename"/>) and the output stream.
        /// </param>
        /// <remarks>
        /// <para>
        /// If the model filename is <c>null</c> or the directory does not exist, the method exits immediately.
        /// Otherwise, it retrieves all files in the directory and uses <see cref="TextureUtils.IsValidTextureFileType"/>
        /// to filter for valid texture file types. For each valid file, the mapper compares its short filename 
        /// (converted to lower-case) with the model file's short filename (also in lower-case). 
        /// When a match is found, it opens the file stream and assigns it to the texture loading context.
        /// </para>
        /// </remarks>
        public override void Map(TextureLoadingContext textureLoadingContext)
        {
            if (textureLoadingContext.Context.Filename == null)
            {
                return;
            }

            var directory = FileUtils.GetFileDirectory(textureLoadingContext.Context.Filename);
            if (Directory.Exists(directory))
            {
                var modelShortFilename = FileUtils.GetShortFilename(textureLoadingContext.Context.Filename).Trim().ToLowerInvariant();
                var files = Directory.GetFiles(directory);
                foreach (var file in files)
                {
                    if (!TextureUtils.IsValidTextureFileType(file))
                    {
                        continue;
                    }

                    var shortFilename = FileUtils.GetShortFilename(file).Trim().ToLowerInvariant();
                    if (modelShortFilename == shortFilename)
                    {
                        textureLoadingContext.Stream = File.OpenRead(file);
                        return;
                    }
                }
            }
        }
    }
}
