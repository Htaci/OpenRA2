using System;
using System.Collections.Generic;
using UnityEngine;

public class JPSPathfinding : MonoBehaviour 
{
    public GridManager gridManager;
    // 节点类
    public class Node : IComparable<Node>
    {
        public int x;
        public int y;
        public int gx; // 实际代价，从起点到当前节点的移动代价
        public int hx; // 预估代价，当前节点到终点的预计代价（启发式）
        public Node parent;

        public Node(int x, int y, int gx, int hx, Node parent)
        {
            this.x = x;
            this.y = y;
            this.gx = gx;
            this.hx = hx;
            this.parent = parent;
        }

        public int fx
        {
            get { return gx + hx; }
        }

        public int CompareTo(Node other)
        {
            int compare = this.fx.CompareTo(other.fx);
            if (compare == 0)
            {
                // 如果f值相同，比较hx
                compare = this.hx.CompareTo(other.hx);
            }
            return compare;
        }

        public override bool Equals(object obj)
        {
            Node other = obj as Node;
            if (other == null) return false;
            return this.x == other.x && this.y == other.y;
        }

        public override int GetHashCode()
        {
            return x * 31 + y;
        }
    }

    // 曼哈顿距离计算（启发式函数）
    private int Heuristic(int x1, int y1, int x2, int y2)
    {
        return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
    }

    // 主寻路函数
    public List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int endPos)
    {
        Node startNode = new Node(startPos.x, startPos.y, 0, Heuristic(startPos.x, startPos.y, endPos.x, endPos.y), null);

        PriorityQueue<Node> openList = new PriorityQueue<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openList.Enqueue(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList.Dequeue();

            if (currentNode.x == endPos.x && currentNode.y == endPos.y)
            {
                // 找到路径
                return RetracePath(currentNode);
            }

            closedSet.Add(currentNode);

            List<Node> successors = IdentifySuccessors(currentNode, endPos);

            foreach (Node successor in successors)
            {
                if (closedSet.Contains(successor))
                    continue;

                if (!openList.Contains(successor))
                {
                    openList.Enqueue(successor);
                }
            }
        }

        // 未找到路径
        return null;
    }

    // 重建路径
    private List<Vector2Int> RetracePath(Node endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(new Vector2Int(currentNode.x, currentNode.y));
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    // 标识后继节点
    private List<Node> IdentifySuccessors(Node node, Vector2Int endPos)
    {
        List<Node> successors = new List<Node>();

        List<Vector2Int> neighborDirs = GetNeighborDirections(node);

        foreach (Vector2Int dir in neighborDirs)
        {
            Node jumpPoint = Jump(node, dir, endPos);
            if (jumpPoint != null)
            {
                int newG = node.gx + Heuristic(node.x, node.y, jumpPoint.x, jumpPoint.y);
                if (jumpPoint.gx == 0 || newG < jumpPoint.gx)
                {
                    jumpPoint.gx = newG;
                    jumpPoint.hx = Heuristic(jumpPoint.x, jumpPoint.y, endPos.x, endPos.y);
                    jumpPoint.parent = node;
                    successors.Add(jumpPoint);
                }
            }
        }

        return successors;
    }

    // 跳点搜索函数
    private Node Jump(Node startNode, Vector2Int dir, Vector2Int endPos)
    {
        Node currentNode = startNode;
        while (true)
        {
            int nextX = currentNode.x + dir.x;
            int nextY = currentNode.y + dir.y;

            // 检查坐标是否可行走
            if (!IsQueryGridMove(nextX, nextY))
                return null;

            Node nextNode = new Node(nextX, nextY, 0, 0, null);

            // 到达终点，返回当前节点作为跳点
            if (nextX == endPos.x && nextY == endPos.y)
                return nextNode;

            // 检查是否有强制邻居
            if (HasForcedNeighbor(nextX, nextY, dir))
                return nextNode;

            // 对角线方向需检查水平和垂直方向是否存在跳点
            if (dir.x != 0 && dir.y != 0)
            {
                // 检查水平方向
                Node horizontal = JumpHorizontal(nextNode, new Vector2Int(dir.x, 0), endPos);
                // 检查垂直方向
                Node vertical = JumpVertical(nextNode, new Vector2Int(0, dir.y), endPos);
                if (horizontal != null || vertical != null)
                    return nextNode;
            }

            // 移动到下一个位置继续检查
            currentNode = nextNode;
        }
    }

    // 水平方向跳跃
    private Node JumpHorizontal(Node startNode, Vector2Int dir, Vector2Int endPos)
    {
        Node currentNode = startNode;
        while (true)
        {
            int nextX = currentNode.x + dir.x;
            int nextY = currentNode.y;

            if (!IsQueryGridMove(nextX, nextY))
                return null;

            Node nextNode = new Node(nextX, nextY, 0, 0, null);

            if (nextX == endPos.x && nextY == endPos.y)
                return nextNode;

            if (HasForcedNeighbor(nextX, nextY, dir))
                return nextNode;

            currentNode = nextNode;
        }
    }

    // 垂直方向跳跃
    private Node JumpVertical(Node startNode, Vector2Int dir, Vector2Int endPos)
    {
        Node currentNode = startNode;
        while (true)
        {
            int nextX = currentNode.x;
            int nextY = currentNode.y + dir.y;

            if (!IsQueryGridMove(nextX, nextY))
                return null;

            Node nextNode = new Node(nextX, nextY, 0, 0, null);

            if (nextX == endPos.x && nextY == endPos.y)
                return nextNode;

            if (HasForcedNeighbor(nextX, nextY, dir))
                return nextNode;

            currentNode = nextNode;
        }
    }


    // 检查是否有强制邻居
    private bool HasForcedNeighbor(int x, int y, Vector2Int dir)
    {
        // 获取当前移动方向的正交方向
        int dx = dir.x;
        int dy = dir.y;

        // 对角线移动
        if (dx != 0 && dy != 0)
        {
            if ((IsQueryGridMove(x - dx, y + dy) && !IsQueryGridMove(x - dx, y)) ||
                (IsQueryGridMove(x + dx, y - dy) && !IsQueryGridMove(x, y - dy)))
            {
                return true;
            }
        }
        else if (dx != 0)
        {
            // 水平移动
            if ((IsQueryGridMove(x + dx, y + 1) && !IsQueryGridMove(x, y + 1)) ||
                (IsQueryGridMove(x + dx, y - 1) && !IsQueryGridMove(x, y - 1)))
            {
                return true;
            }
        }
        else if (dy != 0)
        {
            // 垂直移动
            if ((IsQueryGridMove(x + 1, y + dy) && !IsQueryGridMove(x + 1, y)) ||
                (IsQueryGridMove(x - 1, y + dy) && !IsQueryGridMove(x - 1, y)))
            {
                return true;
            }
        }

        return false;
    }

    // 获取邻居的方向
    private List<Vector2Int> GetNeighborDirections(Node node)
    {
        List<Vector2Int> neighborDirs = new List<Vector2Int>();

        if (node.parent == null)
        {
            // 起始节点，可以朝任何方向进行移动
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    neighborDirs.Add(new Vector2Int(dx, dy));
                }
            }
        }
        else
        {
            // 根据父节点，推导自然邻居方向
            int dx = Mathf.Clamp(node.x - node.parent.x, -1, 1);
            int dy = Mathf.Clamp(node.y - node.parent.y, -1, 1);

            if (dx != 0 && dy != 0)
            {
                // 对角线移动
                if (IsQueryGridMove(node.x, node.y + dy))
                    neighborDirs.Add(new Vector2Int(0, dy));
                if (IsQueryGridMove(node.x + dx, node.y))
                    neighborDirs.Add(new Vector2Int(dx, 0));
                if (IsQueryGridMove(node.x + dx, node.y + dy))
                    neighborDirs.Add(new Vector2Int(dx, dy));
                if (!IsQueryGridMove(node.x - dx, node.y) && IsQueryGridMove(node.x, node.y + dy))
                    neighborDirs.Add(new Vector2Int(-dx, dy));
                if (!IsQueryGridMove(node.x, node.y - dy) && IsQueryGridMove(node.x + dx, node.y))
                    neighborDirs.Add(new Vector2Int(dx, -dy));
            }
            else if (dx == 0)
            {
                // 垂直移动
                if (IsQueryGridMove(node.x, node.y + dy))
                {
                    neighborDirs.Add(new Vector2Int(0, dy));
                    if (!IsQueryGridMove(node.x + 1, node.y))
                        neighborDirs.Add(new Vector2Int(1, dy));
                    if (!IsQueryGridMove(node.x - 1, node.y))
                        neighborDirs.Add(new Vector2Int(-1, dy));
                }
            }
            else if (dy == 0)
            {
                // 水平移动
                if (IsQueryGridMove(node.x + dx, node.y))
                {
                    neighborDirs.Add(new Vector2Int(dx, 0));
                    if (!IsQueryGridMove(node.x, node.y + 1))
                        neighborDirs.Add(new Vector2Int(dx, 1));
                    if (!IsQueryGridMove(node.x, node.y - 1))
                        neighborDirs.Add(new Vector2Int(dx, -1));
                }
            }
        }

        return neighborDirs;
    }

    // 优先级队列的实现
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> data;

        public PriorityQueue()
        {
            this.data = new List<T>();
        }

        public void Enqueue(T item)
        {
            data.Add(item);
            int ci = data.Count - 1; // 子节点索引
            while (ci > 0)
            {
                int pi = (ci - 1) / 2; // 父节点索引
                if (data[ci].CompareTo(data[pi]) >= 0) break; // 如果子节点大于父节点，结束
                T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
                ci = pi;
            }
        }

        public T Dequeue()
        {
            // 假设队列不为空
            int li = data.Count - 1; // 最后一个节点的索引
            T frontItem = data[0];   // 要返回的第一个元素
            data[0] = data[li];
            data.RemoveAt(li);

            --li; // 最后一个节点的索引
            int pi = 0; // 父节点索引
            while (true)
            {
                int ci = pi * 2 + 1; // 左子节点索引
                if (ci > li) break;  // 超出范围
                int rc = ci + 1;     // 右子节点索引
                if (rc <= li && data[rc].CompareTo(data[ci]) < 0)
                    ci = rc;         // 找到更小的子节点
                if (data[pi].CompareTo(data[ci]) <= 0) break; // 父节点小于子节点
                T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp;
                pi = ci;
            }
            return frontItem;
        }

        public int Count
        {
            get { return data.Count; }
        }

        public bool Contains(T item)
        {
            return data.Contains(item);
        }
    }

    // 判断网格是否可行走，可在这里添加针对不同寻路对象的判断，根据实际情况进行修改
    public bool IsQueryGridMove(int x, int y)
    {
        if (gridManager == null)
        {
            gridManager = FindObjectOfType<GridManager>();
        }
        int i = gridManager.GetGridInfo(x,y);
        if (i != -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
