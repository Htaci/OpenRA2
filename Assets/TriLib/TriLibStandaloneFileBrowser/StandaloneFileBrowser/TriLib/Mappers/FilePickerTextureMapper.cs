#pragma warning disable 672

using System;
using System.Collections.Generic;
using TriLibCore.SFB;
using TriLibCore.Interfaces;
using TriLibCore.Utils;
using UnityEngine;

namespace TriLibCore.Mappers
{
    /// <summary>
    /// Provides functionality to load textures from a file picker selection. This mapper searches through the 
    /// list of <see cref="ItemWithStream"/> objects (provided in the custom context data) to find a file whose 
    /// short filename matches the filename specified in the TriLib <see cref="ITexture"/>. If a match is found, 
    /// it opens the corresponding data stream.
    /// </summary>
    [CreateAssetMenu(menuName = "TriLib/Mappers/Texture/File Picker Texture Mapper")]
    public class FilePickerTextureMapper : TextureMapper
    {
        /// <inheritdoc />
        public override void Map(TextureLoadingContext textureLoadingContext)
        {
            if (string.IsNullOrEmpty(textureLoadingContext.Texture.Filename))
            {
                return;
            }
            var itemsWithStream = CustomDataHelper.GetCustomData<IList<ItemWithStream>>(textureLoadingContext.Context.CustomData);
            if (itemsWithStream != null)
            {
                var shortFileName = FileUtils.GetShortFilename(textureLoadingContext.Texture.Filename).Trim().ToLowerInvariant();
                foreach (var itemWithStream in itemsWithStream)
                {
                    if (!itemWithStream.HasData)
                    {
                        continue;
                    }
                    var checkingFileShortName = FileUtils.GetShortFilename(itemWithStream.Name).Trim().ToLowerInvariant();
                    if (shortFileName == checkingFileShortName)
                    {
                        textureLoadingContext.Stream = itemWithStream.OpenStream();
                    }
                }
            }
            else
            {
                Debug.LogWarning("Missing custom context data.");
            }
        }
    }
}
