using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;


/// <summary>
/// 当个 Mod 数据结构，包括了 Mod 的类库，Mod 的编号（Mod的唯一标识），Mod 代码中 Mod类的实例
/// </summary>
public class ModClass
{
    public Assembly assembly { get; set; }
    public int serialNumber { get; set; }
    public object ModInstance { get; set; }
    public Type modType { get; set; }
}

/// <summary>
/// Mod 配置文件
/// </summary>
public class ModConfig
{
    // Mod 名称
    public string modName { get; set; }
    // Mod介绍
    public string modDescription { get; set; }
    // Mod 版本
    public string modVersion { get; set; }
    // Mod图标文件名
    public string modIcon { get; set; }

    // Mod所包含Map地图（以及其部分描述：Map游戏名称，Map任务简报，Map文件名）
    public List<MapInfo> mapFile { get; set; }
    // Mod 中所有模型的文件名
    public List<string> modelFile { get; set; }
    // Mod 中所有音频的文件名
    public List<string> audioFile { get; set; }
    // Mod 中所有图片的文件名
    public List<string> imageFile { get; set; }
}

/// <summary>
/// 地图信息：记录了地图的各种信息
/// </summary>
public class MapInfo
{
    // 地图名称（唯一ID）
    public string mapNameID { get; set; }
    // 地图游戏名称（玩家实际看到的，可以任取）
    public string mapName { get; set; }
    // 地图任务简报
    public string mapTask { get; set; }
    // 地图文件名
    public string mapFileName { get; set; }
    // 任务类型，0为不可见，1为单人任务列表（但仍可联机，在战役列表中）
    // 2为联机地图（属于地图，在遭遇战中选择此地图）
    // 3为合作任务（属于战役，但在联机房间中选择战役模式后可选，但特定地图可单人玩）
    public int taskType { get; set; }
    // 地图是否可以多人合作
    public bool isMultiplayer { get; set; }
    // 地图是否强制多人合作
    public bool isForceMultiplayer { get; set; }
    // 地图是否有子地图
    public bool hasSubMap { get; set; }
    // 地图的子地图列表
    public List<string> subMapList { get; set; }
    // 地图/任务的描述
    public string mapDescription { get; set; }
    // 地图的所有区块信息
    public List<BlockInfo> blockInfoList { get; set; }
    // 地图触发信息
    public List<TriggerInfo> triggerInfoList { get; set; }
    // 地图中的建筑物
    public List<BuildingInfo> buildingInfoList { get; set; }
}


//public class MapDate
//{

//}

/// <summary>
/// 建筑物信息
/// </summary>
public class BuildingInfo
{
    // 建筑物位置
    public Vector2 buildingPosition { get; set; }
    // 建筑物UID
    public string buildingUID { get; set; }
}

/// <summary>
/// 触发器的数据，记录了触发器的各种信息
/// </summary>
public class TriggerInfo
{
    // 触发器名称
    public string triggerName { get; set; }
    // 触发前提
    public string triggerCondition { get; set; }
    // 触发结果
    public string triggerResult { get; set; }
}

/// <summary>
/// 区块的数据，记录了区块的各种信息
/// </summary>
public class BlockInfo
{
    // 该区块内的所有瓦片
    public List<TileInfo> tileInfo { get; set; } = new List<TileInfo>();
}

/// <summary>
/// 单个瓦片的信息
/// </summary>
public class TileInfo
{
    // 瓦片类型，例如草地，石头，水等
    public int tileType { get; set; }
    // 瓦片坐标
    public Vector3 tilePosition { get; set; }
    // 瓦片倾斜
    public int tileSlope { get; set; }
    // 瓦片倾斜指的是瓦片是否为斜坡，平地，悬崖等
}
