// 地图文件信息
using System;

public class MapFileInfo
{
    public string Name { get; set; }          // 地图名称
    public int Width { get; set; }            // 地图宽度
    public int Height { get; set; }           // 地图高度
    public string[] Tiles { get; set; }      // 地图瓦片
}
