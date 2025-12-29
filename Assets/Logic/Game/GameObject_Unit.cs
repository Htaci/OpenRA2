using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有 单位、建筑 的游戏对象都必须添加此脚本
/// </summary>
public class GameObject_Unit : MonoBehaviour
{
    #region 生产权限
    // 生产前提
    public List<string> Prerequisite = new List<string>();
    // 生产所需科技等级,0-10,0代表不限制，负数为不可生产
    public int TechLevel = -1;
    // 建筑白名单：只允许这些阵营建造
    public List<string> RequiredHouses = new List<string>();
    // 建筑黑名单：不允许这些阵营建造
    public List<string> ForbiddenHouses = new List<string>();
    // 建造数量限制，0或负数表示不限制
    public int MaxCount = 0;
    #endregion

    #region 免疫属性
    public bool ProtectedDriver = true; // 驾驶员无敌（驾驶员不能被击杀）
    public bool ProtectedUnit = false;  // 单位无敌（血量锁定）
    public bool CanBeReversed = false;  // 是否免疫迷惑
    public bool Parasiteable = false;   // 是否免疫寄生
    public bool Bombable = false;       // 是否免疫被绑定炸药（如疯狂伊文炸弹，心灵终结中的磁震炸弹）
    public int Crushable = 0;           // 免疫被碾压等级，与碾压等级对应
    public bool ImmuneToPsionics = false;   // 是否免疫心控

    #endregion

    // 单位类型
    enum UnitType
    {
        Building,   // 建筑
        Vehicle,    // 车辆、船
        Infantry,   // 步兵
        Aircraft,   // 飞机
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
