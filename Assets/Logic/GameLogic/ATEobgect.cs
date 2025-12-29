using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 游戏对象更新
public class AteObjectUpdate
{
    // 游戏对象字典（包含了建筑、单位）
    public static Dictionary<int, AteObject> ateObjects = new Dictionary<int, AteObject>();


    public void Update()
    {

    }
}

// 游戏对象
public class AteObject
{
    public Vector2 currentPos;  // 当前坐标
    //public Vector2Int targetPos;   // 目标坐标
    public bool isMoving;          // 是否正在移动
    public List<Vector2Int> path; // 计算出的路径
    private int pathIndex = 0;     // 当前路径节点索引

    // 单位移动更新
    public void MoveUpdate()
    {
        if (isMoving && path != null)
        {
            if (path == null || pathIndex >= path.Count)
            {
                //Debug.Log("路径为空或超出范围.");
                isMoving = false;
                return;
            }

            // 获取当前路径节点，x用于x坐标，y用于z坐标
            Vector2Int target = path[pathIndex];
            Vector2 targetPosition = new Vector2(target.x,  target.y);

            // 向目标节点更新位置移动
            // 如果当前坐标与目标坐标之间的距离小于一次更新的距离，则此次更新视为到达目标点
            if (Vector2.Distance(currentPos, targetPosition) <= 1 * 0.05f)
            {
                currentPos = targetPosition; // 直接将当前坐标设置为目标坐标
                pathIndex++;
            }
            else
            {
                // 更新位置
                currentPos = Vector2.MoveTowards(currentPos, targetPosition, 1 * 0.05f);
            }
        }
    }
}
