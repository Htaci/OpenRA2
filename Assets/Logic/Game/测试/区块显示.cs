using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class 区块显示 : MonoBehaviour
{
    Vector3 mouseWorldPos;
    Vector3 center = new Vector3(0, 0, 0);

    private Material lineMaterial;

    void Start()
    {
        // 创建材质
        lineMaterial = new Material(Shader.Find("Sprites/Default"));
    }

    void OnPostRender()
    {
        // 设置材质
        lineMaterial.SetPass(0);

        Draw();


        GL.End();
    }

    private void Update()
    {
        mouseWorldPos = GetMouseWorldPosition();
        center = GetChunkCenter(mouseWorldPos);
    }

    private void Draw()
    {

        // 开始绘制线条
        Color edgeColor = Color.red;
        edgeColor.a = 1;
        GL.Begin(GL.LINES);
        GL.Color(edgeColor);
        GL.Vertex3(center.x - 6, center.y - 10, center.z - 6); // 起点
        GL.Vertex3(center.x - 6, center.y + 15, center.z - 6); // 终点

        GL.Vertex3(center.x + 6, center.y - 10, center.z - 6); // 起点
        GL.Vertex3(center.x + 6, center.y + 15, center.z - 6); // 终点

        GL.Vertex3(center.x - 6, center.y - 10, center.z + 6); // 起点
        GL.Vertex3(center.x - 6, center.y + 15, center.z + 6); // 终点

        GL.Vertex3(center.x + 6, center.y - 10, center.z + 6); // 起点
        GL.Vertex3(center.x + 6, center.y + 15, center.z + 6); // 终点

        //edgeColor = Color.blue;
        //edgeColor.a = 1;


        //for (int i = 0; i < 9; i++)
        //{
        //    edgeColor = Color.blue;
        //    GL.Color(edgeColor);
        //    GL.Vertex3(center.x - 6, center.y - 10 + i * 4, center.z - 6); // 起点
        //    GL.Vertex3(center.x + 6, center.y - 10 + i * 4, center.z - 6); // 终点

        //    GL.Vertex3(center.x - 6, center.y - 10 + i * 4, center.z + 6); // 起点
        //    GL.Vertex3(center.x - 6, center.y - 10 + i * 4, center.z - 6); // 终点

        //    edgeColor = Color.black;
        //    GL.Color(edgeColor);
        //    GL.Vertex3(center.x - 6, center.y - 10 + i * 4, center.z + 6); // 起点
        //    GL.Vertex3(center.x + 6, center.y - 10 + i * 4, center.z + 6); // 终点

        //    GL.Vertex3(center.x + 6, center.y - 10 + i * 4, center.z + 6); // 起点
        //    GL.Vertex3(center.x + 6, center.y - 10 + i * 4, center.z - 6); // 终点

        //    edgeColor = Color.yellow;
        //    GL.Color(edgeColor);
        //    GL.Vertex3(center.x - 6, center.y - 10 + i * 4 - 2, center.z - 6); // 起点
        //    GL.Vertex3(center.x + 6, center.y - 10 + i * 4 - 2, center.z - 6); // 终点

        //    GL.Vertex3(center.x - 6, center.y - 10 + i * 4 - 2, center.z + 6); // 起点
        //    GL.Vertex3(center.x - 6, center.y - 10 + i * 4 - 2, center.z - 6); // 终点

        //    edgeColor = Color.black;
        //    GL.Color(edgeColor);
        //    GL.Vertex3(center.x - 6, center.y - 10 + i * 4 - 2, center.z + 6); // 起点
        //    GL.Vertex3(center.x + 6, center.y - 10 + i * 4 - 2, center.z + 6); // 终点

        //    GL.Vertex3(center.x + 6, center.y - 10 + i * 4 - 2, center.z + 6); // 起点
        //    GL.Vertex3(center.x + 6, center.y - 10 + i * 4 - 2, center.z - 6); // 终点
        //}

        //edgeColor = Color.blue;
        //GL.Color(edgeColor);
        //GL.Vertex3(center.x - 6, center.y - 10 +  4, center.z - 6); // 起点
        //GL.Vertex3(center.x + 6, center.y - 10 +  4, center.z - 6); // 终点

        //GL.Vertex3(center.x - 6, center.y - 10 + 4, center.z + 6); // 起点
        //GL.Vertex3(center.x - 6, center.y - 10 + 4, center.z - 6); // 终点

        //edgeColor = Color.black;
        //GL.Color(edgeColor);
        //GL.Vertex3(center.x - 6, center.y - 10 + 4, center.z + 6); // 起点
        //GL.Vertex3(center.x + 6, center.y - 10 + 4, center.z + 6); // 终点

        //GL.Vertex3(center.x + 6, center.y - 10 + 4, center.z + 6); // 起点
        //GL.Vertex3(center.x + 6, center.y - 10 + 4, center.z - 6); // 终点

        //edgeColor = Color.yellow;
        //GL.Color(edgeColor);
        //GL.Vertex3(center.x - 6, center.y - 10 + 4 - 2, center.z - 6); // 起点
        //GL.Vertex3(center.x + 6, center.y - 10 + 4 - 2, center.z - 6); // 终点

        //GL.Vertex3(center.x - 6, center.y - 10 + 4 - 2, center.z + 6); // 起点
        //GL.Vertex3(center.x - 6, center.y - 10 + 4 - 2, center.z - 6); // 终点

        //edgeColor = Color.black;
        //GL.Color(edgeColor);
        //GL.Vertex3(center.x - 6, center.y - 10 + 4 - 2, center.z + 6); // 起点
        //GL.Vertex3(center.x + 6, center.y - 10 + 4 - 2, center.z + 6); // 终点

        //GL.Vertex3(center.x + 6, center.y - 10 + 4 - 2, center.z + 6); // 起点
        //GL.Vertex3(center.x + 6, center.y - 10 + 4 - 2, center.z - 6); // 终点





    }

    // 获取鼠标在世界坐标中的位置
    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask mapLayerMask = 1 << 7; // 7:Map 图层的编号
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, mapLayerMask))
        {
            return hitInfo.point;
        }

        return Vector3.zero;
    }

    // 计算鼠标所在区块中心
    public Vector3 GetChunkCenter(Vector3 worldPos)
    {
        // 计算区块索引（自动处理负数）
        int chunkX = Mathf.FloorToInt(worldPos.x / 12f);
        int chunkZ = Mathf.FloorToInt(worldPos.z / 12f);

        // 计算中心点（世界坐标）
        return new Vector3(
            chunkX * 12 + 6f,
            0,
            chunkZ * 12 + 6f
        );
    }
}
