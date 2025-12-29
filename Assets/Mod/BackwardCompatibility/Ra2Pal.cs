using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace RA2ConversionTools
{
    public class PaletteReader
    {
        /// <summary>
        /// 读取一个色盘文件（768字节，256色，RGB格式），返回颜色数组
        /// </summary>
        /// <param name="filePath">色盘文件路径</param>
        /// <returns>颜色数组</returns>
        public static Color[] ReadPalette(string filePath)
        {
            // 256色，每色3字节
            const int colorCount = 256;
            const int bytesPerColor = 3;
            const int paletteSize = colorCount * bytesPerColor;

            byte[] bytes = File.ReadAllBytes(filePath);
            if (bytes.Length < paletteSize)
            {
                Debug.WriteLine("色盘文件长度不足768字节");
                throw new ArgumentException("色盘文件长度不足768字节");
            }

                


            Color[] palette = new Color[colorCount];
            for (int i = 0; i < colorCount; i++)
            {
                int offset = i * bytesPerColor;
                byte r = bytes[offset];
                byte g = bytes[offset + 1];
                byte b = bytes[offset + 2];
                palette[i] = Color.FromArgb(r, g, b);
                // 转化为int输出，用于调试
                Debug.WriteLine($"Color {i}: R={r}, G={g}, B={b}, Int={palette[i].ToArgb() & 0xFFFFFF}");
            }
            return palette;
        }
    }
}
