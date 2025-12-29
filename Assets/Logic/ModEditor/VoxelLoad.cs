using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TriLibCore.Extensions;
using UnityEngine;

public class VxlImporter : MonoBehaviour
{
    public string fpath = "D:\\素材\\单位帧动画\\cplane.vxl";
    public Material voxelMaterial;
    // 测试：色盘文件路径（可在 Inspector 指定），例如 *.pal 文件
    public string palettePath = "";


    public GameObject GameObject; // 父对象
    public GameObject 预制件;

    public int 总体素方块数量 = 0;
    public int 实际加载模型的体素方块数量 = 0;
    public int 被优化剔除掉的体素方块数量 = 0;

    /// <summary>
    /// 记录了加载的vxl模型中所有体素方块的位置（键）和颜色（值）
    /// </summary>
    private Dictionary<Vector3, Color> voxelAllBlock = new Dictionary<Vector3, Color>();


    
    public void Start()
    {
        // 尝试加载指定的色盘（如果指定了）
        if (!string.IsNullOrEmpty(palettePath))
        {
            bool ok = PaletteManager.LoadPalette(palettePath);
            Debug.Log($"尝试加载色盘 '{palettePath}'，结果: {ok}");
        }
        StartCoroutine(加载模型());
    }

    public void Test方块生成(Vector3 position, Color color)
    {
        // 检查预制件和父对象是否已设置
        if (GameObject == null)
        {
            Debug.LogError("VXL加载器：测试脚本：父对象没有设置!");
            return;
        }
        if (预制件 == null)
        {
            Debug.LogError("VXL加载器：测试脚本：预制件没有设置!");
            return;
        }

        // 实例化预制件
        GameObject instance = Instantiate(预制件);

        // 设置为父对象的子对象
        instance.transform.SetParent(GameObject.transform);

        // 设置局部位置
        instance.transform.localPosition = position;

        // 可选：重置局部旋转
        instance.transform.localRotation = Quaternion.identity;

        //Debug.Log("Prefab instance created as child of " + parent.name);
    }

    private IEnumerator 加载模型()
    {
        // 加载vxl数据
        ImportVxl(fpath);
        总体素方块数量 = voxelAllBlock.Count;
        生成体素();

        //融合模型();

        yield return null;
    }

    void 生成体素()
    {
        List<Vector3> 实际需要生成的体素 = new List<Vector3>();
        // 模型顶点，面，颜色数据
        List<VoxelDate> 模型数据 = new List<VoxelDate>();



        foreach (var v in voxelAllBlock)
        {
            bool isVisible = false;

            // 检查这个方块的六个面是否有其他的体素方块
            if (!voxelAllBlock.ContainsKey(new Vector3(v.Key.x + 1, v.Key.y, v.Key.z)) ||
                !voxelAllBlock.ContainsKey(new Vector3(v.Key.x - 1, v.Key.y, v.Key.z)) ||
                !voxelAllBlock.ContainsKey(new Vector3(v.Key.x, v.Key.y + 1, v.Key.z)) ||
                !voxelAllBlock.ContainsKey(new Vector3(v.Key.x, v.Key.y - 1, v.Key.z)) ||
                !voxelAllBlock.ContainsKey(new Vector3(v.Key.x, v.Key.y, v.Key.z + 1)) ||
                !voxelAllBlock.ContainsKey(new Vector3(v.Key.x, v.Key.y, v.Key.z - 1)))
            {
                // 如果任何一个面没有被其他方块包围，则该方块可见
                isVisible = true;
            }

            if (isVisible)
            {
                实际需要生成的体素.Add(v.Key);
            }
            else
            {
                被优化剔除掉的体素方块数量++;
            }
        }

        int batchSize = 8181;
        for (int i = 0; i < 实际需要生成的体素.Count; i += batchSize)
        {
            List<VoxelDate> batch = new List<VoxelDate>();
            int end = Math.Min(i + batchSize, 实际需要生成的体素.Count);
            for (int j = i; j < end; j++)
            {
                batch.Add(CreateVoxelBlock(实际需要生成的体素[j], voxelAllBlock[实际需要生成的体素[j]]));
            }
            融合模型(batch);
        }
    }

    void 融合模型(List<VoxelDate> 模型数据)
    {

        GameObject modelObj = new GameObject("VXL_Model");

        // 设置为父对象的子对象
        modelObj.transform.SetParent(GameObject.transform);

        // 为这个新的对象添加 MeshFilter 网格组件和 MeshRenderer 材质
        MeshFilter meshFilter = modelObj.AddComponent<MeshFilter>();
        MeshRenderer renderer = modelObj.AddComponent<MeshRenderer>();
        // 设置材质为空白
        renderer.material = voxelMaterial;

        // 创建新网格
        meshFilter.mesh = new Mesh();



        meshFilter.mesh.name = "VoxelMesh";
        // 合并顶点数据
        List<Vector3> combinedVertices = new List<Vector3>();
        // 合并面数据
        List<int> combinedTriangles = new List<int>();
        // 合并颜色数据
        List<Color> combinedColors = new List<Color>();

        int vertexOffset = 0; // 顶点偏移量

        foreach (VoxelDate data in 模型数据)
        {
            // 添加顶点数据
            combinedVertices.AddRange(data.verticesDate);

            // 添加面数据，并更新索引
            foreach (int tri in data.trianglesDate)
            {
                combinedTriangles.Add(tri + vertexOffset);
            }

            // 添加颜色数据
            combinedColors.AddRange(data.colorsDate);

            // 更新顶点偏移量
            vertexOffset += data.verticesDate.Length;
        }

        // 将合并后的数据赋值给Mesh
        meshFilter.mesh.vertices = combinedVertices.ToArray();
        meshFilter.mesh.triangles = combinedTriangles.ToArray();
        meshFilter.mesh.colors = combinedColors.ToArray(); // 设置顶点颜色
        Debug.Log("合并顶点总数: " + combinedVertices.Count);
        //mesh.RecalculateNormals(); // 重新计算法线
    }


    public VoxelDate CreateVoxelBlock(Vector3 position, Color color)
    {
        VoxelDate mox = new VoxelDate();

        // 为体素方块网格创建顶点数组
        Vector3[] vertices = new Vector3[8]
        {
        new Vector3(0, 0, 0) + position, // 0 前左下
        new Vector3(1, 0, 0) + position, // 1 前右下
        new Vector3(1, 1, 0) + position, // 2 前右上
        new Vector3(0, 1, 0) + position, // 3 前左上
        new Vector3(0, 0, 1) + position, // 4 后左下
        new Vector3(1, 0, 1) + position, // 5 后右下
        new Vector3(1, 1, 1) + position, // 6 后右上
        new Vector3(0, 1, 1) + position  // 7 后左上
        };

        // 三角形定义（逆时针顺序）
        int[] triangles = new int[36]
        {
        // 前面 (Z=0)
        0, 3, 2,  0, 2, 1, 
        // 右面 (X=1)
        1, 2, 6,  1, 6, 5,
        // 后面 (Z=1)
        5, 6, 7,  5, 7, 4,
        // 左面 (X=0)
        4, 7, 3,  4, 3, 0,
        // 顶面 (Y=1)
        3, 7, 6,  3, 6, 2,
        // 底面 (Y=0)
        4, 0, 1,  4, 1, 5
        };

        // 为每个顶点指定颜色
        Color[] colors = new Color[8];
        for (int i = 0; i < 8; i++)
        {
            colors[i] = color;
        }

        mox.verticesDate = vertices;
        mox.trianglesDate = triangles;
        mox.colorsDate = colors;
        实际加载模型的体素方块数量++;
        //模型数据.Add(mox);
        return mox;
        //Debug.Log($"体素方块：{position}被加载了，颜色是{color}");
    }


    /// <summary>
    /// 加载VXL模型并将体素方块的数据添加到生成字典中
    /// </summary>
    /// <param name="filePath">vxl文件路径</param>
    public void ImportVxl(string filePath)
    {
        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var reader = new VxlReader(fs);
                foreach (var limb in reader.Limbs)
                {
                    ProcessLimb(limb);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("错误输入 VXL 文件: " + ex.Message);
        }
    }

    // private void ProcessLimb(VxlLimb limb)
    // {
    //     for (int x = 0; x < limb.Size[0]; x++)
    //     {
    //         for (int y = 0; y < limb.Size[1]; y++)
    //         {
    //             if (limb.VoxelMap[x, y] != null)
    //             {
    //                 foreach (var kvp in limb.VoxelMap[x, y])
    //                 {
    //                     byte z = kvp.Key;
    //                     var element = kvp.Value;
    //                     Color color = new Color(element.Color / 255f, element.Color / 255f, element.Color / 255f);
    //                     // 将这个方块添加到生成字典中
    //                     voxelAllBlock.Add(new Vector3(x, z, y), color);
    //                     // 生成单个体素模型的网格
    //                     //CreateVoxelBlock(new Vector3(x, z, y), color);
    //                     // 测试用直接实例化方块预制件
    //                     //Test方块生成(new Vector3(x, z, y), color);
    //                 }
    //             }
    //         }
    //     }
    // }
    private void ProcessLimb(VxlLimb limb)
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

                        // 使用调色板索引（element.Color）查找颜色，若没有调色板则回退到原先的灰度计算
                        Color color;
                        int idx = (int)element.Color;
                        if (PaletteManager.Palette != null && idx >= 0 && idx < PaletteManager.Palette.Length)
                        {
                            color = PaletteManager.Palette[idx];
                        }
                        else
                        {
                            // 原来的回退方案（将索引当成灰度）
                            float g = element.Color / 255f;
                            color = new Color(g, g, g, 1f);
                        }

                        // 将这个方块添加到生成字典中
                        voxelAllBlock.Add(new Vector3(x, z, y), color);
                    }
                }
            }
        }
    }
}

public class VoxelDate
{
    public Vector3[] verticesDate { get; set; }
    public int[] trianglesDate { get; set; }
    public Color[] colorsDate { get; set; }
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