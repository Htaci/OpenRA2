using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class 模型创建测试 : MonoBehaviour
{
    private MeshFilter filter;
    private Mesh mesh;

    public int b;

    public Material voxelMaterial;

    // Use this for initialization
    void Start()
    {
        GameObject modelObj = new GameObject("测试_Model");
        modelObj.transform.position = transform.position;

        MeshFilter meshFilter = modelObj.AddComponent<MeshFilter>();
        MeshRenderer renderer = modelObj.AddComponent<MeshRenderer>();

        renderer.material = voxelMaterial;
        // 新建一个mesh
        mesh = new Mesh();
        meshFilter.mesh = mesh;


        List<体素模型数据> 模型数据 = new List<体素模型数据>();
        // 创建两个正方形的顶点数据和面数据
        模型数据.Add(CreateVoxelMesh(new Vector3(0, 0, 0), new Color(1, 0, 0))); // 红色
        模型数据.Add(CreateVoxelMesh(new Vector3(3, 0, 0), new Color(0, 1, 0))); // 绿色
        模型数据.Add(CreateVoxelMesh(new Vector3(0, 3, 0), new Color(0, 0, 1))); // 蓝色
        模型数据.Add(CreateVoxelMesh(new Vector3(0, 0, 3), new Color(1, 1, 0))); // 黄色
        模型数据.Add(CreateVoxelMesh(new Vector3(3, 3, 0), new Color(1, 0, 1))); // 紫色


        创建模型(模型数据);
    }

    void 创建模型(List<体素模型数据> a)
    {
        mesh.name = "MyMesh";
        // 合并顶点数据
        List<Vector3> combinedVertices = new List<Vector3>();
        // 合并面数据
        List<int> combinedTriangles = new List<int>();
        // 合并颜色数据
        List<Color> combinedColors = new List<Color>();

        int vertexOffset = 0; // 顶点偏移量

        foreach (体素模型数据 data in a)
        {
            // 添加顶点数据
            combinedVertices.AddRange(data.顶点数据);

            // 添加面数据，并更新索引
            foreach (int tri in data.面数据)
            {
                combinedTriangles.Add(tri + vertexOffset);
            }

            // 添加颜色数据
            combinedColors.AddRange(data.颜色数据);

            // 更新顶点偏移量
            vertexOffset += data.顶点数据.Length;
        }

        // 将合并后的数据赋值给Mesh
        mesh.vertices = combinedVertices.ToArray();
        mesh.triangles = combinedTriangles.ToArray();
        mesh.colors = combinedColors.ToArray(); // 设置顶点颜色
        //mesh.RecalculateNormals(); // 重新计算法线
    }

    体素模型数据 CreateVoxelMesh(Vector3 position,Color color)
    {
        体素模型数据 mox = new 体素模型数据();
        // 为网格创建顶点数组
        Vector3[] vertices = new Vector3[8]
        {
        new Vector3(0, 0, 0) + position,
        new Vector3(1, 0, 0) + position,
        new Vector3(1, 1, 0) + position,
        new Vector3(0, 1, 0) + position,
        new Vector3(0, 0, 1) + position,
        new Vector3(1, 0, 1) + position,
        new Vector3(1, 1, 1) + position,
        new Vector3(0, 1, 1) + position
        };



        // 通过顶点为网格创建三角形
        int[] triangles = new int[36]{
            // 前
            0, 2, 1, 0, 3, 2,
            // 右
            1, 2, 6, 1, 6, 5,
            // 后
            5, 6, 7, 5, 7, 4,
            // 左
            4, 7, 3, 4, 3, 0,
            // 上
            3, 7, 6, 3, 6, 2,
            // 下
            4, 0, 1, 4, 1, 5
        };

        // 为每个顶点指定颜色
        Color[] colors = new Color[8];
        for (int i = 0; i < 8; i++)
        {
            colors[i] = color;
        }

        mox.顶点数据 = vertices;
        mox.面数据 = triangles;
        mox.颜色数据 = colors;

        return mox;
    }
    
}

public class 体素模型数据 
{
    public Vector3[] 顶点数据 {  get; set; }
    public int[] 面数据 { get; set; }
    public Color[] 颜色数据 { get; set; }
}

