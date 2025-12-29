#pragma warning disable 672

using System;
using ICSharpCode.SharpZipLib.Zip;
using TriLibCore.General;
using TriLibCore.Utils;
using UnityEngine;

namespace TriLibCore.Mappers
{
    /// <summary>
    /// Provides a texture mapping strategy for extracting texture data from Zip files.
    /// This mapper retrieves a Zip file from the custom context data and iterates through its entries,
    /// comparing their names to the texture's filename. If a matching file is found, its stream is opened
    /// and assigned to the texture loading context.
    /// </summary>
    [CreateAssetMenu(menuName = "TriLib/Mappers/Texture/Zip File Texture Mapper")]
    public class ZipFileTextureMapper : TextureMapper
    {
        /// <inheritdoc />
        public override void Map(TextureLoadingContext textureLoadingContext)
        {
            var zipLoadCustomContextData = CustomDataHelper.GetCustomData<ZipLoadCustomContextData>(textureLoadingContext.Context.CustomData);
            if (zipLoadCustomContextData == null)
            {
                return;
            }
            var zipFile = zipLoadCustomContextData.ZipFile;
            if (zipFile == null)
            {
                throw new Exception("Zip file instance is null.");
            }
            if (string.IsNullOrWhiteSpace(textureLoadingContext.Texture.Filename))
            {
                if (textureLoadingContext.Context.Options.ShowLoadingWarnings)
                {
                    Debug.LogWarning("Texture name is null.");
                }
                return;
            }
            // Get the file name (without extension) from the zip entry used for the model
            var modelFilenameWithoutExtension = FileUtils.GetFilenameWithoutExtension(zipLoadCustomContextData.ZipEntry.Name).Trim().ToLowerInvariant();
            // Get the short filename for the texture
            var textureShortName = FileUtils.GetShortFilename(textureLoadingContext.Texture.Filename).Trim().ToLowerInvariant();

            foreach (ZipEntry zipEntry in zipFile)
            {
                if (!zipEntry.IsFile)
                {
                    continue;
                }
                var checkingFileShortName = FileUtils.GetShortFilename(zipEntry.Name).Trim().ToLowerInvariant();
                var checkingFilenameWithoutExtension = FileUtils.GetFilenameWithoutExtension(zipEntry.Name).Trim().ToLowerInvariant();
                if ((TextureUtils.IsValidTextureFileType(checkingFileShortName) &&
                     textureLoadingContext.TextureType == TextureType.Diffuse &&
                     modelFilenameWithoutExtension == checkingFilenameWithoutExtension)
                    || textureShortName == checkingFileShortName)
                {
                    textureLoadingContext.Stream = AssetLoaderZip.ZipFileEntryToStream(out _, zipEntry, zipFile);
                }
            }
        }
    }
}
