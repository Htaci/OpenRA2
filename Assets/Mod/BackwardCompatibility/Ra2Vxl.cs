using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RA2ResourceKit
{

    public class VXLRead
    {

        // 使用说明，你可以先通过 ImportVxl 方法读取一个 VXL 文件，得到一个 VxlReader 对象
        // 然后将其（VxlReader）传入 GetAllVoxels 方法获得一个包含所有体素方块信息的字典
        // 可选：你可以使用 CullInvisibleFaces 方法来剔除不可见面的体素方块，优化渲染性能（当然了按照现代计算机的性能，它的优化可能聊胜于无）


        /// <summary>
        /// 读取VXL模型，返回一个 VxlReader 对象
        /// </summary>
        /// <param name="filePath">vxl文件路径</param>
        public static VxlReader ImportVxl(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var reader = new VxlReader(fs);
                    return reader;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("错误输入 VXL 文件: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获得一个包含了所有体素方块信息的字典
        /// <summary>
        public Dictionary<Vector3, Color> GetAllVoxels(VxlReader vxlreader)
        {
            var voxelDict = new Dictionary<Vector3, Color>();
            foreach (var limb in vxlreader.Limbs)
            {
                for (int x = 0; x < limb.Size[0]; x++)
                {
                    for (int y = 0; y < limb.Size[1]; y++)
                    {
                        if (limb.VoxelMap[x, y] != null)
                        {
                            foreach (var kvp in limb.VoxelMap[x, y])
                            {
                                byte z = kvp.Key;
                                var element = kvp.Value;
                                // 使用 Color.FromArgb 创建颜色对象
                                var colorValue = element.Color;
                                Color color = Color.FromArgb(colorValue, colorValue, colorValue);
                                // 将这个方块添加到生成字典中
                                voxelDict.Add(new Vector3(x, z, y), color);
                            }
                        }
                    }
                }
            }

            return voxelDict;
        }

        /// <summary>
        /// 一个可以剔除不可见面的体素方块的方法，返回被剔除的体素方块数量
        /// </summary>
        public static int CullInvisibleFaces(ref Dictionary<Vector3, Color> voxelAllBlock)
        {
            // 被优化剔除掉的体素方块数量
            int culledCount = 0;
            // 创建一个临时列表来存储需要移除的方块的键

            var keysToRemove = new List<Vector3>();

            foreach (var v in voxelAllBlock)
            {
                bool isVisible = false;

                // 检查这个方块的六个面是否有其他的体素方块
                if (!voxelAllBlock.ContainsKey(new Vector3(v.Key.X + 1, v.Key.Y, v.Key.Z)) ||
                    !voxelAllBlock.ContainsKey(new Vector3(v.Key.X - 1, v.Key.Y, v.Key.Z)) ||
                    !voxelAllBlock.ContainsKey(new Vector3(v.Key.X, v.Key.Y + 1, v.Key.Z)) ||
                    !voxelAllBlock.ContainsKey(new Vector3(v.Key.X, v.Key.Y - 1, v.Key.Z)) ||
                    !voxelAllBlock.ContainsKey(new Vector3(v.Key.X, v.Key.Y, v.Key.Z + 1)) ||
                    !voxelAllBlock.ContainsKey(new Vector3(v.Key.X, v.Key.Y, v.Key.Z - 1)))
                {
                    // 如果任何一个面没有被其他方块包围，则该方块可见
                    isVisible = true;
                }

                // 如果不可见，则将其添加到 keysToRemove 列表，稍后统一移除
                if (!isVisible)
                {
                    keysToRemove.Add(v.Key);
                    culledCount++;
                }
            }

            // 统一移除所有不可见的方块
            foreach (var key in keysToRemove)
            {
                voxelAllBlock.Remove(key);
            }

            return culledCount;
        }
    }

    public enum NormalType { TiberianSun = 2, RedAlert2 = 4 }
    public class VxlElement
    {
        public byte Color;
        public byte Normal;
    }

    public class VxlLimb
    {
        public string Name;
        public float Scale;
        public float[] Bounds;
        public byte[] Size;
        public NormalType Type;

        public uint VoxelCount;
        public Dictionary<byte, VxlElement>[,] VoxelMap;
    }

    public class VxlReader
    {
        public readonly uint LimbCount;
        public VxlLimb[] Limbs;

        uint bodySize;

        static void ReadVoxelData(Stream s, VxlLimb l)
        {
            var baseSize = l.Size[0] * l.Size[1];
            var colStart = new int[baseSize];
            for (var i = 0; i < baseSize; i++)
                colStart[i] = s.ReadInt32();
            s.Seek(4 * baseSize, SeekOrigin.Current);
            var dataStart = s.Position;

            l.VoxelCount = 0;
            for (var i = 0; i < baseSize; i++)
            {
                if (colStart[i] == -1) continue;

                s.Seek(dataStart + colStart[i], SeekOrigin.Begin);
                var z = 0;
                do
                {
                    z += s.ReadUInt8();
                    var count = s.ReadUInt8();
                    z += count;
                    l.VoxelCount += count;
                    s.Seek(2 * count + 1, SeekOrigin.Current);
                } while (z < l.Size[2]);
            }

            l.VoxelMap = new Dictionary<byte, VxlElement>[l.Size[0], l.Size[1]];
            for (var i = 0; i < baseSize; i++)
            {
                if (colStart[i] == -1) continue;

                s.Seek(dataStart + colStart[i], SeekOrigin.Begin);

                var x = (byte)(i % l.Size[0]);
                var y = (byte)(i / l.Size[0]);
                byte z = 0;
                l.VoxelMap[x, y] = new Dictionary<byte, VxlElement>();
                do
                {
                    z += s.ReadUInt8();
                    var count = s.ReadUInt8();
                    for (var j = 0; j < count; j++)
                    {
                        var v = new VxlElement();
                        v.Color = s.ReadUInt8();
                        v.Normal = s.ReadUInt8();

                        l.VoxelMap[x, y].Add(z, v);
                        z++;
                    }

                    s.ReadUInt8();
                } while (z < l.Size[2]);
            }
        }

        public VxlReader(Stream s)
        {
            if (!s.ReadASCII(16).StartsWith("Voxel Animation"))
                throw new InvalidDataException("Invalid vxl header");

            s.ReadUInt32();
            LimbCount = s.ReadUInt32();
            s.ReadUInt32();
            bodySize = s.ReadUInt32();
            s.Seek(770, SeekOrigin.Current);

            Limbs = new VxlLimb[LimbCount];
            for (var i = 0; i < LimbCount; i++)
            {
                Limbs[i] = new VxlLimb();
                Limbs[i].Name = s.ReadASCII(16);
                s.Seek(12, SeekOrigin.Current);
            }

            s.Seek(802 + 28 * LimbCount + bodySize, SeekOrigin.Begin);

            var limbDataOffset = new uint[LimbCount];
            for (var i = 0; i < LimbCount; i++)
            {
                limbDataOffset[i] = s.ReadUInt32();
                s.Seek(8, SeekOrigin.Current);
                Limbs[i].Scale = s.ReadFloat();
                s.Seek(48, SeekOrigin.Current);

                Limbs[i].Bounds = new float[6];
                for (var j = 0; j < 6; j++)
                    Limbs[i].Bounds[j] = s.ReadFloat();
                Limbs[i].Size = s.ReadBytes(3);
                Limbs[i].Type = (NormalType)s.ReadByte();
            }

            for (var i = 0; i < LimbCount; i++)
            {
                s.Seek(802 + 28 * LimbCount + limbDataOffset[i], SeekOrigin.Begin);
                ReadVoxelData(s, Limbs[i]);
            }
        }
    }

    public static class StreamExtensions
    {
        public static byte[] ReadBytes(this Stream stream, int count)
        {
            byte[] buffer = new byte[count];
            int bytesRead = stream.Read(buffer, 0, count);
            if (bytesRead != count)
                throw new EndOfStreamException($"Requested {count} bytes but got {bytesRead}");
            return buffer;
        }

        public static byte ReadUInt8(this Stream stream)
        {
            return (byte)stream.ReadByte();
        }

        public static int ReadInt32(this Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static uint ReadUInt32(this Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public static float ReadFloat(this Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return BitConverter.ToSingle(buffer, 0);
        }

        public static string ReadASCII(this Stream stream, int length)
        {
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, length);
            return Encoding.ASCII.GetString(buffer);
        }
    }
}
