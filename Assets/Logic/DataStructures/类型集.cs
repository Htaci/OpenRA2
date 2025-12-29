using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 所有单位属性的数据结构
public class S_UnitUID
{
    public string Name { get; set; } // 单位名字
    public float TotalTime { get; set; } // 建造所需时间
    public string Type { get; set; } // 单位类型 
                                     // Building(建筑)、DefensiveBuilding(防御性建筑)、Infantry(步兵)
                                     // ArmyArmor(陆军装甲)、ArmyHeavyArmor(陆军重型装甲)
                                     // NavalArmor(海军装甲)

    // 如果是建筑类型，则属性为占地格子，如果不是，则不生效，除建筑单位，所有单位都占地1格


    public List<string> AllowProduction { get; set; } // 允许生产什么，当拥有此单位时，可以生产列表中的单位
    public List<string> Prerequisite { get; set; } // 先决条件，如果为空，有单位AllowProduction提供且科技等级满足即可建造
                                                   // 如果包含了某些单位，则即便 AllowProduction 提供了生产权限，也需要拥有 Prerequisite 内包含的单位才可以生产
    public int TechLevel { get; set; } // 科技等级 任何非0-10之间（包含0和10）的数都为不可建造，例如-1
                                       // 在 AllowProduction 和 Prerequisite 都满足的情况下，也必须满足TechLevel限制，这是必选项，如果不设置则无法建造
    public string Owner { get; set; } = "All";// 归属阵营
                                              // 暂无配置，默认为 All(全部)
    public string RequiredHouses { get; set; } // 为空不生效，此选项规定只允许某阵营生产
    public string ForbiddenHouses { get; set; } // 为空不生效，此选项规定不允许某单位
    public string BuildLimit { get; set; } // 限制建造数量，默认为空不限制
    public int Cost { get; set; } = 0;// 生产所需价格,默认为0



}
// 支援技能
public class S_SupportSkills
{
    public string Name { get; set; } // 技能名字
    public float TotalTime { get; set; } // 充能所需时间
    public string Type { get; set; } // 技能类型 
}

// 模组包数据结构
public class S_ModPack
{
    // 模组包名字
    public string packName { get; set; }
    // 模组包介绍
    public string maindate { get; set; }
    // 单位属性字典，记录了该包内所有单位的属性
    public Dictionary<string, S_UnitUID> unitUID = new Dictionary<string, S_UnitUID>();
    // 资源

    public MemoryStream memoryStream;
    // 图片资源
    public Sprite sprite;
}

// main.json数据格式
public class S_Main_json
{
    // 单位属性配置文件 的文件名字
    public List<string> filestring = new List<string>();
    // 武器属性配置文件 的文件名字
    public List<string> data = new List<string>();
    // 资源属性配置文件 的文件名字（资源包括了：3D模型资源、图片资源、视频等）
}