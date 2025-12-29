using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 鼠标世界位置和寻路路径 : MonoBehaviour
{
    Vector3 mouseWorldPos;
    Vector3 center = new Vector3(0, 0, 0);

    public static List<Vector2Int> pathtest; // 单位寻路的路径点

    public static bool 寻路绘制测试 = true;

    private Material lineMaterial;

    [Header("路径绘制设置")]
    public float pathLineHeight = 0.2f; // 路径绘制高度
    public Color pathColor = new Color(0, 0.8f, 1f, 1f); // 青蓝色路径

    void Start()
    {
        // 创建材质
        lineMaterial = new Material(Shader.Find("Sprites/Default"));
    }

    void OnPostRender()
    {
        lineMaterial.SetPass(0);
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector3 gridPosition = new Vector3(Mathf.Round(mouseWorldPos.x), Mathf.Round(mouseWorldPos.y), Mathf.Round(mouseWorldPos.z));

        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        GL.Vertex3(mouseWorldPos.x, mouseWorldPos.y, mouseWorldPos.z); // 起点
        GL.Vertex3(mouseWorldPos.x, mouseWorldPos.y + 1.5f, mouseWorldPos.z); // 终点

        GL.Color(Color.green);
        GL.Vertex3(gridPosition.x, gridPosition.y - 1, gridPosition.z); // 起点
        GL.Vertex3(gridPosition.x, gridPosition.y + 1.5f, gridPosition.z); // 终点

        GL.End();

        // 新增路径绘制部分
        DrawPathLines();
    }

    void DrawPathLines()
    {
        if (pathtest == null || pathtest.Count < 2) return;

        GL.Begin(GL.LINES);
        lineMaterial.SetPass(0);
        GL.Color(pathColor);

        for (int i = 0; i < pathtest.Count - 1; i++)
        {


            Vector3 start = new (pathtest[i].x, 0 , pathtest[i].y);
            Vector3 end = new(pathtest[i+1].x, 0, pathtest[i+1].y);

            // 绘制水平连线
            GL.Vertex(start + Vector3.up * pathLineHeight);
            GL.Vertex(end + Vector3.up * pathLineHeight);

            // 绘制垂直标记（可选）
            GL.Vertex(start + Vector3.up * (pathLineHeight - 0.1f));
            GL.Vertex(start + Vector3.up * (pathLineHeight + 0.1f));
        }

        GL.End();
    }

    // Update is called once per frame
    void Update()
    {

    }



    // 获取鼠标在世界坐标中的位置
    private Vector3 GetMouseWorldPosition()
    {
        // 创建一条从摄像机到鼠标位置的射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 创建一个LayerMask，用于指定射线检测的图层，这里指定的是Map图层
        LayerMask mapLayerMask = 1 << 7; // 7:Map 图层的编号
        // 如果射线检测到碰撞，则返回碰撞点的位置
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, mapLayerMask))
        {
            return hitInfo.point;
        }

        // 如果射线没有检测到碰撞，则返回Vector3.zero
        return new Vector3(0, 9999, 0);
    }
}
