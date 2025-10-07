using System.Numerics;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace GameCore
{
    /// <summary>
    /// 用于寻路的静态类，包含了对A*和JPS算法的使用
    /// </summary>
    public static class Pathfinding
    {
        // 寻路方法，使用JPS算法
        public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
        {
            return JPSPathfinding.FindPath(start, end);
        }
        // 寻路方法，使用A*算法
        public static List<Vector2Int> FindPathAStar(Vector2Int start, Vector2Int end)
        {
            return AStar.FindPath(start, end);
        }
        /// <summary>
        /// 获取游戏单位移动更新后的位置
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Vector3 UpdateMovement(GameObject obj)
        {
            if (!obj.IsMovementPaused)
            {
                if (obj.path == null || obj.pathIndex >= obj.path.Count)
                {
                    //Debug.Log("路径为空或超出范围.");
                    obj.IsMovementPaused = false;
                    return new Vector3();
                }
                
                // 获取当前路径目标节点
                Vector2Int target = obj.path[obj.pathIndex];
                // 这里的Y并未计算，还没有添加高度计算逻辑，因为现在的测试地图是平的，之后会更新
                Vector3 targetPosition = new Vector3(target.x, obj.position.Y, target.y);

                // 移动到目标位置，每固定逻辑帧更新时触发，移动速度为单位的移动速度*每帧更新速度
                obj.position = Mathf.MoveTowards(obj.position, targetPosition, obj.MoveSpeed * 0.05f);

                // 如果达到目标节点，前进到下一个节点
                if (Vector3.Distance(obj.position, targetPosition) < 0.01f)
                {
                    obj.pathIndex++;
                }
            }
            return obj.position;
        }



    }

    /// <summary>
    /// A*寻路算法实现，在该项目中，用于针对Jps路径上出现频繁变动的障碍物时，进行局部路径重新计算到下一个跳点
    /// ，当然了，对于Mod开发者，也可以直接使用A*算法进行完整路径计算，不过效率会低一些
    /// </summary>
    public static class AStar
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
        public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
        {
            // Test:创建 Stopwatch 对象来计时
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
                    Console.WriteLine($"耗时{stopwatch.ElapsedMilliseconds} ms");
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
            Console.WriteLine($"耗时{stopwatch.ElapsedMilliseconds} ms");

            return null; // 如果没有路径
        }

        // 获取当前节点到目标的启发式估算值（曼哈顿距离）
        private static int GetHeuristic(Node current, Node goal)
        {
            return Mathf.Abs(current.x - goal.x) + Mathf.Abs(current.y - goal.y);
        }

        // 获取邻居节点（八个方向：上、下、左、右、左上、右上、左下、右下）
        private static List<Node> GetNeighbors(Node node, Node goal)
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
        public static bool IsQueryGridMove(int x, int y)
        {
            // 这里假设可以走，返回true
            return true;
        }
    }


    /// <summary>
    /// JPS寻路算法实现，基于A*算法的优化版本，适用于网格地图的路径查找，效率更高，本项目中用于单位的主要寻路算法
    /// </summary>
    public static class JPSPathfinding
    {
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
        private static int Heuristic(int x1, int y1, int x2, int y2)
        {
            return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
        }

        // 主寻路函数
        public static List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int endPos)
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
        private static List<Vector2Int> RetracePath(Node endNode)
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
        private static List<Node> IdentifySuccessors(Node node, Vector2Int endPos)
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
        private static Node Jump(Node startNode, Vector2Int dir, Vector2Int endPos)
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
        private static Node JumpHorizontal(Node startNode, Vector2Int dir, Vector2Int endPos)
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
        private static Node JumpVertical(Node startNode, Vector2Int dir, Vector2Int endPos)
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
        private static bool HasForcedNeighbor(int x, int y, Vector2Int dir)
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
        private static List<Vector2Int> GetNeighborDirections(Node node)
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
        public static bool IsQueryGridMove(int x, int y)
        {

            int i = GridManager.GetGridInfo(x, y);
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


    /// <summary>
    /// 整数二维坐标，用于网格寻路
    /// </summary>
    public struct Vector2Int
    {
        public int x { get; set; }
        public int y { get; set; }

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        // 可选：重载相等和哈希
        public override bool Equals(object obj)
        {
            return obj is Vector2Int other && x == other.x && y == other.y;
        }
        public override int GetHashCode() => x * 31 + y;
    }

    /// <summary>
    /// 数学工具类，提供常用静态方法，兼容Unity的Mathf用法
    /// </summary>
    public static class Mathf
    {
        public static int Abs(int value) => value < 0 ? -value : value;
        public static float Abs(float value) => value < 0f ? -value : value;

        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        /// <summary>
        /// 从current移动到target，最大移动距离为maxDistanceDelta
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="maxDistanceDelta"></param>
        /// <returns></returns>
        public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            Vector3 delta = target - current;
            float distance = delta.Length();
            if (distance <= maxDistanceDelta || distance == 0f)
                return target;
            return current + delta / distance * maxDistanceDelta;
        }
    }
}