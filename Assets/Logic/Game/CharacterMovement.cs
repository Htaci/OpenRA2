using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterMovement : MonoBehaviour
{
    //public Vector2Int currentPos;  // 当前坐标
    //public Vector2Int targetPos;   // 目标坐标
    public bool isMoving;          // 是否正在移动
    public List<Vector2Int> path; // 计算出的路径
    private int pathIndex = 0;     // 当前路径节点索引
    private AStar aStar;           // A*算法实例
    private JPSPathfinding jPSPathfinding;

    void Start()
    {
        aStar = FindObjectOfType<AStar>(); // 获取A*实例
        jPSPathfinding = FindObjectOfType<JPSPathfinding>();

        //JPS();
        //AstarPathfinding();
    }

    //void Update()
    //{
    //    if (isMoving && path != null)
    //    {
    //        MoveAlongPath();
    //    }

    //    if (鼠标世界位置和寻路路径.寻路绘制测试)
    //    {
    //        if (鼠标世界位置和寻路路径.pathtest != path)
    //        {
    //            鼠标世界位置和寻路路径.pathtest = path;
    //        }
    //    }
    //}

    void FixedUpdate()
    {
        if (isMoving && path != null)
        {
            MoveAlongPath();
        }

        if (鼠标世界位置和寻路路径.寻路绘制测试)
        {
            if (鼠标世界位置和寻路路径.pathtest != path)
            {
                鼠标世界位置和寻路路径.pathtest = path;
            }
        }
    }

    //调用A* 算法计算路径
    public void AstarPathfinding(Vector2Int currentPos, Vector2Int targetPos)
    {
        if (aStar != null)
        {
            path = aStar.FindPath(currentPos, targetPos);
            pathIndex = 0; // 重置路径索引
        }
    }
    // 调用Jps 算法计算路径
    public void JpsPathfinding(Vector2Int currentPos, Vector2Int targetPos)
    {
        // Debug.Log("开始位置：" + currentPos + "结束位置：" + targetPos);
        if (jPSPathfinding != null)
        {
            path = jPSPathfinding.FindPath(currentPos, targetPos);
            if (path == null)
            {
                Debug.LogError("JPS 寻路失败.");

            }
            if (path != null)
            {
                //foreach (Vector2Int point in path)
                //{
                //    Debug.Log($"路径点: {point}");
                //}
            }
            pathIndex = 0; // 重置路径索引
        }
    }


    private void MoveAlongPath()
    {
        //Debug.LogError("被执行了2");
        if (path == null || pathIndex >= path.Count)
        {
            //Debug.Log("路径为空或超出范围.");
            isMoving = false;
            return;
        }

        // 获取当前路径节点，x用于x坐标，y用于z坐标
        Vector2Int target = path[pathIndex];
        Vector3 targetPosition = new Vector3(target.x, transform.position.y, target.y);

        // 移动到目标位置 Update
        //transform.position = Vector3.MoveTowards(transform.position, targetPosition, 1 * Time.deltaTime);

        // 移动到目标位置 FixedUpdate
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, 1 * 0.05f);

        // 如果达到目标节点，前进到下一个节点
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            pathIndex++;
            //Debug.Log("达到节点，正在移动到下个节点");
        }
    }

    public Vector3 GetGamePosition()
    {
        return transform.position;
    }
}
