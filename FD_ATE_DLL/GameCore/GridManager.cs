public static class GridManager
{
    // 使用 Dictionary 来存储网格信息，Key 是 (x, y) 坐标，Value 是网格的属性（字符串）
    private static Dictionary<Tuple<int, int>, int> gridData = new Dictionary<Tuple<int, int>, int>();

    // 更新或插入网格信息的方法
    public static void UpdateGrid(int x, int y, int info)
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
    public static int GetGridInfo(int x, int y)
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
    public static void PrintAllGridData()
    {
        foreach (var kvp in gridData)
        {
            Console.WriteLine($"Grid ({kvp.Key.Item1}, {kvp.Key.Item2}): Info = {kvp.Value}");
        }
    }


}
