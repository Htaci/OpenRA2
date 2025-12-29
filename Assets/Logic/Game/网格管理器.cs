using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public enum 网格属性
{
    TestGround,
    TestAirWall,

}

public class GridManager : MonoBehaviour
{
    // 使用 Dictionary 来存储网格信息，Key 是 (x, y) 坐标，Value 是网格的属性（字符串）
    private Dictionary<Tuple<int, int>, int> gridData = new Dictionary<Tuple<int, int>, int>();

    public static GridManager Instance;

    public Material Material;

    void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        更新地图();
        
        
    }
    public void 更新地图()
    {
        // 遍历当前物体下所有的子物体
        foreach (Transform child in transform)
        {
            // 获取子物体的位置
            Vector3 position = child.position;

            // 提取出 x 和 z 坐标
            int x = Mathf.RoundToInt(position.x);
            int y = Mathf.RoundToInt(position.z); // 注意这里的 y 代表的是 z 坐标

            // 调用 UpdateGrid 方法更新字典
            UpdateGrid(x, y, 1); // 1 表示可以行走
        }
        PrintAllGridData();
        Debug.Log("更新地图字典");
    }

    // 更新或插入网格信息的方法
    public void UpdateGrid(int x, int y, int info)
    {
        // 创建坐标的键
        var key = new Tuple<int, int>(x, y);

        // 如果字典中已经包含这个坐标，更新其值；否则插入新的键值对
        if (gridData.ContainsKey(key))
        {
            gridData[key] = info;
        }
        else
        {
            gridData.Add(key, info);
        }
    }

    // 根据坐标查询网格信息的方法
    public int GetGridInfo(int x, int y)
    {
        var key = new Tuple<int, int>(x, y);

        // 如果字典中包含该坐标，返回对应的属性；否则返回默认值
        if (gridData.TryGetValue(key, out int value))
        {
            return value;
        }
        else
        {
            return -1; // 如果没有该网格，返回-1
        }
    }

    // 打印所有网格信息，方便调试
    public void PrintAllGridData()
    {
        //foreach (var entry in gridData)
        //{
        //    //Debug.Log($"Grid ({entry.Key.Item1}, {entry.Key.Item2}) Info: {entry.Value}");
        //}
    }

    /// <summary>
    /// 创建地图区块网格数据
    /// </summary>
    public void CreateBlockMesh(BlockInfo blockInfo)
    {

        GameObject modelObj = new GameObject("区块测试_Model");
        // 设置标签为Map
       // modelObj.tag = "Map";
        modelObj.layer = 7;
        //LayerMask mapLayerMask = 1 << 7; // 7:Map 图层的编号
        modelObj.transform.position = new Vector3(-0.5f, 0, -0.5f);

        MeshFilter meshFilter = modelObj.AddComponent<MeshFilter>();
        MeshRenderer renderer = modelObj.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = modelObj.AddComponent<MeshCollider>();
        

        renderer.material = Material;
        // 新建一个mesh
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        

        List<TileMeshDate> tileDate = new List<TileMeshDate>();

        foreach (var s in blockInfo.tileInfo)
        {
            tileDate.Add(CreateTileMesh(s.tilePosition));
        }


        mesh.name = "MyMesh";
        // 合并顶点数据
        List<Vector3> combinedVertices = new List<Vector3>();
        // 合并面数据
        List<int> combinedTriangles = new List<int>();
        // 合并颜色数据
        //List<Color> combinedColors = new List<Color>();

        int vertexOffset = 0; // 顶点偏移量

        foreach (TileMeshDate data in tileDate)
        {
            // 添加顶点数据
            combinedVertices.AddRange(data.verticesDate);

            // 添加面数据，并更新索引
            foreach (int tri in data.trianglesDate)
            {
                combinedTriangles.Add(tri + vertexOffset);
            }


            // 更新顶点偏移量
            vertexOffset += data.verticesDate.Length;
        }
        
        // 将合并后的数据赋值给Mesh
        mesh.vertices = combinedVertices.ToArray();
        mesh.triangles = combinedTriangles.ToArray();

        // 关键点：计算法线
        mesh.RecalculateNormals();   // 让 Unity 根据面自动算

        meshCollider.sharedMesh = mesh;
    }

    // 创建区块中单个瓦片的网格
    public TileMeshDate CreateTileMesh(Vector3 vector3)
    {
        TileMeshDate tileDate = new TileMeshDate();
        // 创建一个正方型的顶点并附上三角形
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0) + vector3,
            new Vector3(1, 0, 0)+ vector3,
            new Vector3(0, 0, 1)+ vector3,
            new Vector3(1, 0, 1)+ vector3,
        };

        // 为这个正方形创建三角形
        int[] tris = new int[6]
        {
            
            0, 2, 1, // 下
            2, 3, 1 
        };
        tileDate.verticesDate = vertices;
        tileDate.trianglesDate = tris;
        //tileDate.colorsDate = colors;

        return tileDate;
    }
}

public class TileMeshDate
{
    /// <summary>
    /// 顶点数据
    /// </summary>
    public Vector3[] verticesDate { get; set; }
    /// <summary>
    /// 面数据
    /// </summary>
    public int[] trianglesDate { get; set; }
    /// <summary>
    /// 颜色数据
    /// </summary>
    public Color[] colorsDate { get; set; }
}
