using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RA2ResourceKit
{
    // 读取 Map 文件并解码
    public static class MapRead
    {
        // 读取入口，传入Map文件路径，使用方法如 
        // MapRead.ReadMain("文件路径");
        // 这个方法没有返回值，可以自己写个返回值把地图信息返回回去使用，同时的，目前的这个版本不支持加载其他数据
        public static void ReadMain(string mapPath)
        {
            // 定义并读取基础的参数，Map文件为Base64编码
            byte[] isoMapPack5Bytes; // 地图瓦片数据（未处理）
            string isoMapPack5Base64; // 地图瓦片数据（实际使用的），表头为 [IsoMapPack5]
            string[] mapSize = MapSectionExtractor.Extract(mapPath, out isoMapPack5Bytes, out isoMapPack5Base64); // 地图基础数据，有地图大小，地图环境，限制可见大小三个属性，表头为 [Map]

            Debug.WriteLine($"IsoMapPack5 Base64 数据: {isoMapPack5Base64}");
            Debug.WriteLine($"IsoMapPack5 字节数组长度: {isoMapPack5Bytes.Length}");
            Debug.WriteLine($"地图尺寸: {mapSize}");

            // 地图的宽高
            int Width = Int32.Parse(mapSize[0]);
            int Height = Int32.Parse(mapSize[1]);

            int cells = (Width * 2 - 1) * Height;
            IsoTile[,] Tiles = new IsoTile[Width * 2 - 1, Height];

            // 解码Base64
            byte[] lzoData = Convert.FromBase64String(isoMapPack5Base64);
            int lzoPackSize = cells * 11 + 4;
            var isoMapPack = new byte[lzoPackSize]; // 解压后的数据缓冲区
            var decodeBuffer = new DecodeBuffer(lzoData, isoMapPack); // 解压数据缓冲区
            uint 总解压大小 = MapSectionExtractor.DecodeInto(decodeBuffer); // 解压缩
            // 打印
            Debug.WriteLine($"解压缩后数据长度: {总解压大小}, 预期长度: {lzoPackSize}，decodeBuffer内Desr长度: {decodeBuffer.Dest}");
            Debug.WriteLine($"decodeBuffer.Src.Length: {decodeBuffer.Src.Length}");
            Debug.WriteLine($"decodeBuffer.Dest.Length: {decodeBuffer.Dest.Length}");
            Debug.WriteLine($"decodeBuffer.Dest 前20字节: {BitConverter.ToString(decodeBuffer.Dest.Take(20).ToArray())}");
            isoMapPack = decodeBuffer.Dest;


            List<IsoTile> Tile_input_list = new List<IsoTile>();

            // 用BitConverter逐块读取
            for (int i = 0; i < cells; i++)
            {
                int offset = i * 11;    // 每块11字节
                ushort rx = BitConverter.ToUInt16(isoMapPack, offset);
                ushort ry = BitConverter.ToUInt16(isoMapPack, offset + 2);
                short tilenum = BitConverter.ToInt16(isoMapPack, offset + 4);
                short zero1 = BitConverter.ToInt16(isoMapPack, offset + 6);
                byte subtile = isoMapPack[offset + 8];
                byte z = isoMapPack[offset + 9];
                byte zero2 = isoMapPack[offset + 10];

                int dx = rx - ry + Width - 1;
                int dy = rx + ry - Width - 1;
                if (dx >= 0 && dx < 2 * Width && dy >= 0 && dy < 2 * Height)
                {
                    var tile = new IsoTile((ushort)dx, (ushort)dy, rx, ry, z, tilenum, subtile);
                    Tile_input_list.Add(tile);  // 添加到列表
                }
            }

            // 测试输出
            foreach (var tile in Tile_input_list)
            {
                Console.WriteLine($"tile瓷砖的x是({tile.Rx}),y是({tile.Ry}),高度是({tile.Z})，编号是({tile.TileNum})，子编号是({tile.SubTile})");
            }
            Console.WriteLine($"共有 {Tile_input_list.Count} 块砖");

            
            MapDate tilesToSave = new MapDate();
            // 地图大小
            tilesToSave.mapSizeWidth = Width;
            tilesToSave.mapSizeHeight = Height;
            // 地图环境
            tilesToSave.Theater = mapSize[2];
            // 限制可见区域
            tilesToSave.LocalSize = new int[] { Int32.Parse(mapSize[3]), Int32.Parse(mapSize[4]), Int32.Parse(mapSize[5]), Int32.Parse(mapSize[6]) };
            // 地图瓦片列表
            foreach (var tile in Tile_input_list)
            {
                // 这里调整了顺序适应unity的坐标，在ra中，xy代表水平坐标，z是高度，在unity中，xz是水平坐标，y才是高度 ，Rx与Ry也是反着的
                tilesToSave.vector3andNum.Add(new int[] { tile.Ry, tile.Z, tile.Rx, tile.TileNum, tile.SubTile });
            }


            // 序列化
            var json = JsonSerializer.Serialize(tilesToSave, new JsonSerializerOptions
            {
                WriteIndented = true   // 缩进
            });

            // 保存到 Assets/tiles.json
            Path.GetFileName(mapPath);
            var path = Path.Combine("Assets", $"{Path.GetFileNameWithoutExtension(mapPath)}.json");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!); // 若目录不存在
            File.WriteAllText(path, json);

            // 并且在资源管理器中显示
            // 弹出资源管理器并定位到文件
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"/select,\"{path}\"",
                UseShellExecute = true
            });

            Debug.WriteLine($"已保存 {tilesToSave.vector3andNum.Count} 块砖到 {path}");
        }
    }


    // 导出时使用的单个瓦片信息,未来会支持如建筑，单位等信息的提取
    public class MapDate
    {
        public int mapSizeWidth { get; set; }
        public int mapSizeHeight { get; set; }
        // 地图环境
        public string Theater { get; set; }
        // 限制可见区域
        public int[] LocalSize { get; set; } = new int[4];
        // 地图瓦片列表
        public List<int[]> vector3andNum { get; set; } = new List<int[]>(); // 前三位是xzy，四五位是TileNum和SubTile
    }

    /// <summary>
    /// 用于储存压缩信息与解压信息的类，方便传递，Src是压缩的数据（原始数据），Dest是存放解压数据的（我们需要的数据）
    /// Dest需要预先分配好空间，以及最终需要的数据是Dest
    /// </summary>
    public class DecodeBuffer
    {
        public byte[] Src { get; }
        public byte[] Dest { get; }

        public DecodeBuffer(byte[] src, byte[] dest)
        {
            Src = src;
            Dest = dest;
        }
    }


    // 读取Map数据并解码
    public static class MapSectionExtractor
    {
        /// <summary>
        /// 读取地图文件，提取 IsoMapPack5 的 Base64 数据并解码为 byte[]，
        /// 同时返回 Map 区块的详细信息。
        /// </summary>
        /// <param name="mapPath">.yrm / .map 文件完整路径</param>
        /// <param name="base64Bytes">输出的解码后二进制数据</param>
        /// <param name="isoMapPack5Base64">输出的 IsoMapPack5 Base64 数据</param>
        /// <returns>Map 区块的详细信息数组</returns>
        public static string[] Extract(string mapPath, out byte[] base64Bytes, out string isoMapPack5Base64)
        {
            if (!File.Exists(mapPath))
                throw new FileNotFoundException("Map file not found", mapPath);

            var lines = File.ReadAllLines(mapPath, Encoding.UTF8); // 显式使用 UTF-8 编码

            // 1. 找到所有 [Section] 及其行号
            var sectionLines = new List<(string Name, int Index)>();
            for (int i = 0; i < lines.Length; i++)
            {
                var trimmed = lines[i].Trim();
                if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                    sectionLines.Add((trimmed, i));
            }

            // 2. 提取 IsoMapPack5 的 Base64 字符串
            isoMapPack5Base64 = ExtractSectionBase64(lines, sectionLines, "[IsoMapPack5]");
            // 3. 解码
            base64Bytes = Convert.FromBase64String(isoMapPack5Base64);

            // 4. 提取 Map 区块的详细信息
            return ExtractSectionValues(lines, sectionLines, "[Map]");
        }


        /// <summary>提取[Map]区块的内容,关于这里，在原版中，这个区块的格式是
        /// [Map]
        /// Theater=URBAN
        /// Size = 0,0,50,64
        /// LocalSize=2,6,46,52
        /// 而在心灵终结的地图中，这个区块的格式却是
        /// [Map]
        /// Size=0,0,150,160
        /// Theater=SNOW
        /// LocalSize = 2,4,146,151
        /// 
        /// 因此这里是读取整个区块并找到Size所对应的值，使其可以兼容两个版本
        /// 
        /// Size指地图大小，Theater指地图环境，如SNOW实际上指Sno，在原版中Sno是寒带环境（雪地），而URBAN指的是Urb，即城市环境，
        /// LocalSize指地图可见大小，用于限制摄像机边缘
        /// [0][1]指的是地图的宽Width和高Height，[2]指Theater 即地图环境，[3] [4] [5] [6]分别指LocalSize 的四个值
        /// </summary>
        /// <param name="lines">文件的所有行</param>
        /// <param name="sections">所有区块及其行号</param>
        /// <param name="target">目标区块名称</param>
        /// <returns>Map 区块的详细信息数组</returns>
        private static string[] ExtractSectionValues(string[] lines, List<(string Name, int Index)> sections, string target)
        {
            int idx = sections.FindIndex(s => s.Name.Equals(target, StringComparison.OrdinalIgnoreCase));
            if (idx == -1) throw new KeyNotFoundException($"Section {target} not found");

            int start = sections[idx].Index + 1;
            int end = (idx + 1 < sections.Count) ? sections[idx + 1].Index : lines.Length;

            string size = null;
            string theater = null;
            string localSize = null;

            for (int i = start; i < end; i++)
            {
                var line = lines[i].Trim();
                if (line.StartsWith("Size=", StringComparison.OrdinalIgnoreCase))
                {
                    var eq = line.IndexOf('=');
                    size = eq >= 0 ? line.Substring(eq + 1).Trim() : line;
                }
                else if (line.StartsWith("Theater=", StringComparison.OrdinalIgnoreCase))
                {
                    var eq = line.IndexOf('=');
                    theater = eq >= 0 ? line.Substring(eq + 1).Trim() : line;
                }
                else if (line.StartsWith("LocalSize=", StringComparison.OrdinalIgnoreCase))
                {
                    var eq = line.IndexOf('=');
                    localSize = eq >= 0 ? line.Substring(eq + 1).Trim() : line;
                }
            }

            if (size == null || theater == null || localSize == null)
                throw new KeyNotFoundException($"One or more keys not found in section {target}");

            // Parse Size (only the last two values)
            string[] sizeParts = size.Split(',');
            if (sizeParts.Length < 4)
                throw new FormatException("Invalid format for Size");

            // Parse LocalSize
            string[] localSizeParts = localSize.Split(',');
            if (localSizeParts.Length < 4)
                throw new FormatException("Invalid format for LocalSize");

            // Create the result array
            string[] result = new string[7];
            result[0] = sizeParts[2];  // Width
            result[1] = sizeParts[3];  // Height
            result[2] = theater;       // Theater
            result[3] = localSizeParts[0]; // 下面四个分别是限制可见区域的四个点
            result[4] = localSizeParts[1];
            result[5] = localSizeParts[2];
            result[6] = localSizeParts[3];

            return result;
        }

        /// <summary>
        /// 提取指定区块的 Base64 合并字符串。
        /// </summary>
        private static string ExtractSectionBase64(string[] lines, List<(string Name, int Index)> sections, string target)
        {
            int idx = sections.FindIndex(s => s.Name.Equals(target, StringComparison.OrdinalIgnoreCase));
            if (idx == -1) throw new KeyNotFoundException($"Section {target} not found");

            int start = sections[idx].Index + 1;
            int end = (idx + 1 < sections.Count) ? sections[idx + 1].Index : lines.Length;

            var sb = new StringBuilder();
            for (int i = start; i < end; i++)
            {
                var line = lines[i].Trim();
                if (line.Length == 0) continue;

                // 去除行首的 "数字=" 前缀
                var eq = line.IndexOf('=');
                sb.Append(eq >= 0 ? line.Substring(eq + 1) : line);
            }

            // 补末尾可能缺失的 '='（Base64 填充）
            int missing = (4 - (sb.Length % 4)) % 4;
            sb.Append('=', missing);

            return sb.ToString();
        }

        /// <summary>
        /// 读取 DecodeBuffer 中的压缩数据 Src，使用解压算法解压到 Dest 中，
        /// </summary>
        /// <param name="decodeBuffer"></param>
        /// <returns></returns>
        public static uint DecodeInto(DecodeBuffer decodeBuffer)
        {
            int r = 0;
            int w = 0;
            int w_end = decodeBuffer.Dest.Length;
            int srcLength = decodeBuffer.Src.Length;
            int i = 0;

            while (w < w_end)
            {
                if (r + 4 > srcLength) break; // 防止越界
                ushort size_in = BitConverter.ToUInt16(decodeBuffer.Src, r);
                r += 2;
                uint size_out = BitConverter.ToUInt16(decodeBuffer.Src, r);
                r += 2;

                if (size_in == 0 || size_out == 0)
                    break;
                if (r + size_in > srcLength || w + size_out > w_end)
                    break; // 防止越界

                // 设置当前块的视图
                var blockBuffer = new DecodeBuffer(
                            decodeBuffer.Src,  // 使用原始数组
                            decodeBuffer.Dest  // 使用原始数组
                        );

                // 修改 Decompress 方法，让它知道从哪里开始读写
                DecompressWithOffset(blockBuffer, r, w, (int)size_in, (int)size_out, out uint actual_out);

                r += size_in;
                w += (int)size_out;
                i++;
            }
            Debug.WriteLine("While" + i);
            return (uint)w;
        }

        /// <summary>
        /// 用于解压红色警戒2地图数据的算法实现，负责解压表头为 [IsoMapPack5] 的数据块。
        /// </summary>
        /// <param name="decodeBuffer"></param>
        /// <param name="inputOffset"></param>
        /// <param name="outputOffset"></param>
        /// <param name="inputLength"></param>
        /// <param name="outputLength"></param>
        /// <param name="size_out"></param>
        public static void DecompressWithOffset(DecodeBuffer decodeBuffer, int inputOffset, int outputOffset, int inputLength, int outputLength, out uint size_out)
        {
            byte[] input = decodeBuffer.Src;
            byte[] output = decodeBuffer.Dest;

            int ip = inputOffset;  // 从指定偏移开始
            int op = outputOffset; // 从指定偏移开始
            int inputEnd = inputOffset + inputLength;
            int outputEnd = outputOffset + outputLength;

            bool gt_first_literal_run = false;
            bool gt_match_done = false;
            uint t;

            Debug.WriteLine($"开始解压: inputOffset={inputOffset}, outputOffset={outputOffset}, inputLength={inputLength}, outputLength={outputLength}");

            // 处理第一个字面量运行
            if (ip < inputEnd && input[ip] > 17)
            {
                t = (uint)(input[ip++] - 17);
                if (t < 4)
                {
                    while (t > 0 && ip < inputEnd && op < outputEnd)
                    {
                        output[op++] = input[ip++];
                        t--;
                    }
                    if (ip < inputEnd) t = input[ip++];
                }
                else
                {
                    while (t > 0 && ip < inputEnd && op < outputEnd)
                    {
                        output[op++] = input[ip++];
                        t--;
                    }
                    gt_first_literal_run = true;
                }
            }

            while (op < outputEnd && ip < inputEnd)
            {
                if (gt_first_literal_run)
                {
                    gt_first_literal_run = false;
                    goto first_literal_run;
                }

                t = input[ip++];
                if (t >= 16) goto match;

                // 字面量拷贝处理
                if (t == 0)
                {
                    while (ip < inputEnd && input[ip] == 0)
                    {
                        t += 255;
                        ip++;
                    }
                    if (ip < inputEnd) t += (uint)(15 + input[ip++]);
                }

                // 4字节块拷贝
                for (int i = 0; i < 4 && ip < inputEnd && op < outputEnd; i++)
                {
                    output[op++] = input[ip++];
                }

                if (--t > 0)
                {
                    while (t > 0 && ip < inputEnd && op < outputEnd)
                    {
                        output[op++] = input[ip++];
                        t--;
                    }
                }

            first_literal_run:
                if (ip >= inputEnd) break;
                t = input[ip++];
                if (t >= 16) goto match;

                // 短距离匹配拷贝
                int m_pos = op - (1 + 0x0800);
                m_pos -= (int)(t >> 2);
                if (ip < inputEnd) m_pos -= input[ip++] << 2;

                // 边界检查的匹配拷贝
                for (int i = 0; i < 3 && m_pos >= 0 && m_pos < output.Length && op < outputEnd; i++)
                {
                    if (m_pos < output.Length)
                        output[op++] = output[m_pos++];
                }
                gt_match_done = true;

            match:
                do
                {
                    if (gt_match_done)
                    {
                        gt_match_done = false;
                        goto match_done;
                    }

                    if (t >= 64)
                    {
                        m_pos = op - 1;
                        m_pos -= (int)((t >> 2) & 7);
                        if (ip < inputEnd) m_pos -= input[ip++] << 3;
                        t = (uint)((t >> 5) - 1);

                        // 安全拷贝
                        for (int i = 0; i < 2 && m_pos >= 0 && m_pos < output.Length && op < outputEnd; i++)
                        {
                            if (m_pos < output.Length)
                                output[op++] = output[m_pos++];
                        }
                        while (t > 0 && m_pos >= 0 && m_pos < output.Length && op < outputEnd)
                        {
                            if (m_pos < output.Length)
                                output[op++] = output[m_pos++];
                            t--;
                        }
                        goto match_done;
                    }
                    else if (t >= 32)
                    {
                        t &= 31;
                        if (t == 0)
                        {
                            while (ip < inputEnd && input[ip] == 0)
                            {
                                t += 255;
                                ip++;
                            }
                            if (ip < inputEnd) t += (uint)(31 + input[ip++]);
                        }

                        m_pos = op - 1;
                        if (ip + 1 < inputEnd)
                        {
                            ushort offset = BitConverter.ToUInt16(input, ip);
                            m_pos -= offset >> 2;
                            ip += 2;
                        }
                    }
                    else if (t >= 16)
                    {
                        m_pos = op;
                        m_pos -= (int)((t & 8) << 11);
                        t &= 7;
                        if (t == 0)
                        {
                            while (ip < inputEnd && input[ip] == 0)
                            {
                                t += 255;
                                ip++;
                            }
                            if (ip < inputEnd) t += (uint)(7 + input[ip++]);
                        }

                        if (ip + 1 < inputEnd)
                        {
                            ushort offset = BitConverter.ToUInt16(input, ip);
                            m_pos -= offset >> 2;
                            ip += 2;
                        }

                        if (m_pos == op) goto eof_found;
                        m_pos -= 0x4000;
                    }
                    else
                    {
                        m_pos = op - 1;
                        m_pos -= (int)(t >> 2);
                        if (ip < inputEnd) m_pos -= input[ip++] << 2;

                        for (int i = 0; i < 2 && m_pos >= 0 && m_pos < output.Length && op < outputEnd; i++)
                        {
                            if (m_pos < output.Length)
                                output[op++] = output[m_pos++];
                        }
                        goto match_done;
                    }

                    // 执行匹配拷贝
                    for (int i = 0; i < 2 && m_pos >= 0 && m_pos < output.Length && op < outputEnd; i++)
                    {
                        if (m_pos < output.Length)
                            output[op++] = output[m_pos++];
                    }
                    while (t > 0 && m_pos >= 0 && m_pos < output.Length && op < outputEnd)
                    {
                        if (m_pos < output.Length)
                            output[op++] = output[m_pos++];
                        t--;
                    }

                match_done:
                    if (ip - 2 >= inputOffset && ip - 2 < inputEnd)
                    {
                        t = (uint)(input[ip - 2] & 3);
                        if (t == 0) break;

                        for (int i = 0; i < t && ip < inputEnd && op < outputEnd; i++)
                        {
                            output[op++] = input[ip++];
                        }
                    }
                    if (ip < inputEnd) t = input[ip++];
                } while (ip < inputEnd && op < outputEnd);
            }

        eof_found:
            size_out = (uint)(op - outputOffset);
            Debug.WriteLine($"解压完成: 实际输出大小 = {size_out}");
        }

    }
 
    /// <summary>
    /// 单个瓦片数据
    /// </summary>
    public class IsoTile
    {
        public ushort Dx;   // 原版中用于辅助绘制的坐标X，坐标是渲染坐标（屏幕）
        public ushort Dy;   // 同上，是渲染坐标Y
        public ushort Rx;   // 实际坐标X
        public ushort Ry;   // 实际坐标Y
        public byte Z;      // 1 bytes，高度
        public short TileNum;   // 16-bit TileNum与SubTile共同决定了瓦片的类型与样式
        public byte SubTile;    // 1 bytes 但这里我在解决shp的问题vxl的也解决了，等shp解决后，我就会着手准备这个TileNum与SubTile的对应素材表

        public IsoTile(ushort p1, ushort p2, ushort rx, ushort ry, byte z, short tilenum, byte subtile)
        {
            Dx = p1;
            Dy = p2;
            Rx = rx;
            Ry = ry;
            Z = z;
            TileNum = tilenum;
            SubTile = subtile;
        }

    }

}
