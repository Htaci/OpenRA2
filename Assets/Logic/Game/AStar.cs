using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Diagnostics;

public class AStar : MonoBehaviour
{
    // 节点
    private class Node
    {
        public int x, y;
        public int g, h, f; // g: 从起点到当前节点的代价，h: 当前节点到目标节点的估计代价，f: g + h
        public Node parent;

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    // A*算法的核心方法
    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        // 创建 Stopwatch 对象来计时
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();  // 启动计时器
        // 开放列表和关闭列表
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        Node startNode = new Node(start.x, start.y);
        Node goalNode = new Node(goal.x, goal.y);

        startNode.g = 0;
        startNode.h = GetHeuristic(startNode, goalNode);
        startNode.f = startNode.g + startNode.h;

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // 从开放列表中选择f值最小的节点
            Node currentNode = openList[0];
            foreach (var node in openList)
            {
                if (node.f < currentNode.f)
                    currentNode = node;
            }

            // 如果当前节点是目标节点，构建路径
            if (currentNode.x == goalNode.x && currentNode.y == goalNode.y)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                while (currentNode != null)
                {
                    path.Add(new Vector2Int(currentNode.x, currentNode.y));
                    currentNode = currentNode.parent;
                }
                path.Reverse();

                stopwatch.Stop();  // 如果有路径，停止计时
                UnityEngine.Debug.Log($"耗时{stopwatch.ElapsedMilliseconds} ms");
                return path; // 返回路径
            }

            // 移除开放列表中的当前节点，加入关闭列表
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // 获取邻居节点
            List<Node> neighbors = GetNeighbors(currentNode, goalNode);
            foreach (var neighbor in neighbors)
            {
                // 如果邻居在关闭列表中，跳过
                if (closedList.Contains(neighbor))
                    continue;

                // 如果邻居不可走，跳过
                if (!IsQueryGridMove(neighbor.x, neighbor.y))
                    continue;

                // 计算g值和f值
                int tentativeG = currentNode.g + 1;
                if (tentativeG < neighbor.g || !openList.Contains(neighbor))
                {
                    neighbor.parent = currentNode;
                    neighbor.g = tentativeG;
                    neighbor.h = GetHeuristic(neighbor, goalNode);
                    neighbor.f = neighbor.g + neighbor.h;

                    // 如果邻居不在开放列表中，则加入
                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }

        stopwatch.Stop();  // 停止计时
        UnityEngine.Debug.Log($"耗时{stopwatch.ElapsedMilliseconds} ms");

        return null; // 如果没有路径
    }

    // 获取当前节点到目标的启发式估算值（曼哈顿距离）
    private int GetHeuristic(Node current, Node goal)
    {
        return Mathf.Abs(current.x - goal.x) + Mathf.Abs(current.y - goal.y);
    }

    // 获取邻居节点（八个方向：上、下、左、右、左上、右上、左下、右下）
    private List<Node> GetNeighbors(Node node, Node goal)
    {
        List<Node> neighbors = new List<Node>();

        // 八个方向的变化
        int[] dirsX = { 0, 0, -1, 1, -1, 1, -1, 1 };
        int[] dirsY = { -1, 1, 0, 0, -1, -1, 1, 1 };

        for (int i = 0; i < 8; i++)
        {
            int newX = node.x + dirsX[i];
            int newY = node.y + dirsY[i];

            neighbors.Add(new Node(newX, newY));
        }

        return neighbors;
    }

    // 判断xy目标网格是否可以行走
    public bool IsQueryGridMove(int x, int y)
    {
        // 这里假设可以走，返回true
        return true;
    }
}
