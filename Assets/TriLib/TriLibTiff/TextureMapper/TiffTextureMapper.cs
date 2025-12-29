using System;
using TriLibCore.Mappers;
using TriLibCore.Textures;
using UnityEngine;

namespace TriLibCore.Tiff
{
    /// <summary>
    /// Represents a TextureMapper that handles loading and processing of TIFF file textures.
    /// </summary>
    /// <remarks>
    /// Changelog: 10/22/2024 - Implemented ReadRGBAImageOriented which is guaranteed to load the correct data in any TIFF configuration.
    /// </remarks>

    [CreateAssetMenu(menuName = "TriLib/Mappers/Texture/Tiff Texture Mapper")]
    public class TiffTextureMapper : TextureMapper
    {
        /// <summary>
        /// Tries to load the TextureLoadingContext texture as a TIFF texture.
        /// </summary>
        /// <param name="textureLoadingContext">The context containing all the Texture data.</param>
        public override void Map(TextureLoadingContext textureLoadingContext)
        {
            if (textureLoadingContext.HasValidData)
            {
                return;
            }
            textureLoadingContext.Texture.ResolveFilename(textureLoadingContext.Context);
            if (textureLoadingContext.Texture.ResolvedFilename != null)
            {
                using (var image = BitMiracle.LibTiff.Classic.Tiff.Open(textureLoadingContext.Texture.ResolvedFilename, "r"))
                {
                    if (image == null)
                    {
                        return;
                    }
                    ProcessTiffData(textureLoadingContext, image);
                }
            }
            if (!textureLoadingContext.HasValidData && textureLoadingContext.HasValidEmbeddedDataStream)
            {
                using (var image = BitMiracle.LibTiff.Classic.Tiff.ClientOpen("InMemory", "r", textureLoadingContext.Texture.DataStream, new BitMiracle.LibTiff.Classic.TiffStream()))
                {
                    if (image == null)
                    {
                        return;
                    }
                    ProcessTiffData(textureLoadingContext, image);
                }
            }
        }

        /// <summary>
        /// Fills the TextureLoadingContext data with the TIFF image data.
        /// </summary>
        /// <param name="textureLoadingContext">The context containing all the Texture data.</param>
        /// <param name="image">The processed TIFF image.</param>
        private static void ProcessTiffData(TextureLoadingContext textureLoadingContext, BitMiracle.LibTiff.Classic.Tiff image)
        {
            var width = image.GetField(BitMiracle.LibTiff.Classic.TiffTag.IMAGEWIDTH)[0].ToInt();
            var height = image.GetField(BitMiracle.LibTiff.Classic.TiffTag.IMAGELENGTH)[0].ToInt();
            var raster = new int[width * height];
            image.ReadRGBAImageOriented(width, height, raster, BitMiracle.LibTiff.Classic.Orientation.BOTLEFT);
            var rawData = new byte[raster.Length * 4];
            var rawDataIndex = 0;
            for (var i = 0; i < raster.Length; i++)
            {
                var value = raster[i];
                rawData[rawDataIndex++] = (byte)(value & 0xFF); 
                rawData[rawDataIndex++] = (byte)((value >> 8) & 0xFF);
                rawData[rawDataIndex++] = (byte)((value >> 16) & 0xFF);
                rawData[rawDataIndex++] = (byte)((value >> 24) & 0xFF);
            }
            textureLoadingContext.RawData = rawData;
            textureLoadingContext.Width = width;
            textureLoadingContext.Height = height;
            textureLoadingContext.CreationBitsPerChannel = 8;
            textureLoadingContext.Components = 4;
        }
    }
}