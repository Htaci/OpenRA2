using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RA2ConversionTools
{
    /// <summary>
    /// Westwood SHP（精灵）文件的读取器。
    /// </summary>
    public static class ShpReader
    {
        /// <summary>
        /// 读取Westwood的SHP文件，并返回每帧的原始调色板索引数组列表。
        /// </summary>
        /// <param name="filePath">.shp文件的路径。</param>
        /// <returns>字节数组列表，每个字节数组表示一帧的像素数据（调色板索引）。</returns>
        public static List<byte[]> ReadShp(string filePath)
        {
            var frames = new List<byte[]>();

            using (var reader = new BinaryReader(File.OpenRead(filePath)))
            {
                // 读取文件头
                ushort numFrames = reader.ReadUInt16();

                // 某些SHP版本，接下来的两个字节未使用。
                // 其他版本可能是更复杂头的一部分。
                // 对于经典C&C SHP，可以跳过。
                reader.ReadUInt16(); // 未使用或保留

                ushort width = reader.ReadUInt16();
                ushort height = reader.ReadUInt16();

                // 这些通常也是头的一部分，但基本帧提取时可能不需要。
                ushort numImagesPerFrame = reader.ReadUInt16(); // 通常为1
                reader.ReadUInt16(); // 未使用

                int frameSize = width * height;
                var frameOffsets = new uint[numFrames];
                var frameDataOffsets = new uint[numFrames];

                // 读取帧偏移和重映射信息
                for (int i = 0; i < numFrames; i++)
                {
                    frameOffsets[i] = reader.ReadUInt32();
                }

                // 每帧的实际数据在偏移表之后。
                // 有些SHP文件在主偏移后有额外的重映射信息，
                // 但简单提取时可直接读取帧数据。
                // 通过第一个偏移计算帧数据起始位置。
                long dataStartOffset = frameOffsets[0];

                for (int i = 0; i < numFrames; i++)
                {
                    // 头中的偏移通常是相对于数据块起始，
                    // 但很多实现显示偏移是从文件开始。这里假定从文件开始。
                    reader.BaseStream.Seek(frameOffsets[i], SeekOrigin.Begin);

                    // 每帧有自己的RLE编码头
                    ushort rleOffset = reader.ReadUInt16(); // RLE数据相对帧起始的偏移
                    byte rleType = reader.ReadByte();      // RLE压缩类型
                    byte rleFlags = reader.ReadByte();     // RLE标志

                    // 跳转到该帧RLE数据起始
                    reader.BaseStream.Seek(frameOffsets[i] + rleOffset, SeekOrigin.Begin);

                    var frameData = new byte[frameSize];
                    int index = 0;

                    // 解码RLE数据
                    while (index < frameSize)
                    {
                        byte b = reader.ReadByte();
                        if (b == 0)
                        {
                            // 包类型0：下一个字节指定透明像素数量。
                            byte count = reader.ReadByte();
                            if (count == 0)
                            {
                                // count为0是特殊情况，表示“行结束”。
                                // 对齐到下一行起始。
                                index = ((index / width) + 1) * width;
                            }
                            else
                            {
                                // 添加count个透明像素（值为0）。
                                index += count;
                            }
                        }
                        else
                        {
                            // 包类型1：该字节本身指定要复制的像素数量。
                            byte count = b;
                            for (int j = 0; j < count; j++)
                            {
                                if (index < frameSize)
                                {
                                    frameData[index++] = reader.ReadByte();
                                }
                                else
                                {
                                    // 数据溢出，停止读取
                                    break;
                                }
                            }
                        }
                    }
                    frames.Add(frameData);
                }
            }
            return frames;
        }
    }
}
