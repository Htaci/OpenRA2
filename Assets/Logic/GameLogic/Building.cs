using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Production
{
    // 生产字典
    public static Dictionary<int, PlayerGroup> ProductionDictionary = new Dictionary<int, PlayerGroup>();

    // 生产更新
    public static void UpdateProduction()
    {
        foreach (var item in ProductionDictionary)
        {
            // ProductionAgreement 层
            for (int j = 0; j < ProductionDictionary[item.Key].ProductionsList.Count; j++)
            {
                
                // 主要建筑生产更新
                if (ProductionDictionary[item.Key].ProductionsList[j].ProductionA.Building != null) // 如果当前正在生产的单位内容不为空
                {
                    //Debug.LogWarning($"建造管理器:{item.Key}的第{j}组的主要建筑不为空");
                    // 判断是否 是已经完成生产了
                    if (!ProductionDictionary[item.Key].ProductionsList[j].ProductionA.Building.IsProduction)
                    {
                        // 判断是否没有被暂停
                        if (ProductionDictionary[item.Key].ProductionsList[j].ProductionA.IsSuspend)
                        {
                            // 设置该栏被占用
                            //Production[item.Key].ProductionsList[j].ProductionA.IsProduction = true;
                            // 生产进度增加
                            ProductionDictionary[item.Key].ProductionsList[j].ProductionA.Building.Schedule += 0.05f;
                            //Debug.Log($"建造管理器:主要建筑 {Production[item.Key].ProductionsList[j].ProductionA.Building.BuildingUID} 生产进度+0.05，总进度:{Production[item.Key].ProductionsList[j].ProductionA.Building.Schedule}");
                        }
                        else
                        {
                            Debug.LogWarning($"建造管理器:{item.Key}的第{j}组的主要建筑被暂停了");
                        }
                        // 如果生产完成
                        if (ProductionDictionary[item.Key].ProductionsList[j].ProductionA.Building.Schedule >= ProductionDictionary[item.Key].ProductionsList[j].ProductionA.Building.TotalTime)
                        {
                            Debug.Log($"建造管理器:主要建筑 {ProductionDictionary[item.Key].ProductionsList[j].ProductionA.Building.BuildingUID} 生产完成，总进度:{ProductionDictionary[item.Key].ProductionsList[j].ProductionA.Building.Schedule}");
                            // 暂停生产
                            ProductionDictionary[item.Key].ProductionsList[j].ProductionA.IsSuspend = false;
                            // 设置生产完成
                            ProductionDictionary[item.Key].ProductionsList[j].ProductionA.Building.IsProduction = true;
                            //IconListManager.Instance.isIconlistUpdate = true;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"建造管理器:{item.Key}的第{j}组不为空，已经完成");
                    }
                }
                else
                {
                    //Debug.LogWarning($"建造管理器:{item.Key}的第{j}组的主要建筑为空");
                }
                //else if (Production[item.Key].ProductionsList[j].ProductionA.Building == null) // 如果当前正在生产的单位内容为空
                //{
                //    // 取消占用栏
                //    Production[item.Key].ProductionsList[j].ProductionA.IsProduction = false;
                //}

                // 防御建筑生产更新
                if (ProductionDictionary[item.Key].ProductionsList[j].ProductionB.Building != null) // 如果当前正在生产的单位内容不为空
                {
                    if (!ProductionDictionary[item.Key].ProductionsList[j].ProductionB.Building.IsProduction)
                    {
                        if (!ProductionDictionary[item.Key].ProductionsList[j].ProductionB.IsSuspend)
                        {
                            ProductionDictionary[item.Key].ProductionsList[j].ProductionB.IsProduction = true;
                            ProductionDictionary[item.Key].ProductionsList[j].ProductionB.Building.Schedule += 0.05f;
                        }
                        // 如果生产完成
                        if (ProductionDictionary[item.Key].ProductionsList[j].ProductionB.Building.Schedule >= ProductionDictionary[item.Key].ProductionsList[j].ProductionB.Building.TotalTime)
                        {
                            // 暂停生产
                            ProductionDictionary[item.Key].ProductionsList[j].ProductionB.IsSuspend = true;
                        }
                    }
                }
                else if (ProductionDictionary[item.Key].ProductionsList[j].ProductionB.Building == null) // 如果当前正在生产的单位内容为空
                {
                    // 取消占用栏
                    ProductionDictionary[item.Key].ProductionsList[j].ProductionB.IsProduction = false;
                }

                // 步兵单位生产更新
                if (ProductionDictionary[item.Key].ProductionsList[j].ProductionC.unitProductions.Count != 0)
                {
                    if (!ProductionDictionary[item.Key].ProductionsList[j].ProductionC.unitProductions[0].IsProduction)
                    {
                        if (!ProductionDictionary[item.Key].ProductionsList[j].ProductionC.IsSuspend)
                        {

                            ProductionDictionary[item.Key].ProductionsList[j].ProductionC.unitProductions[0].Schedule += 0.05f;
                            Debug.Log($"单位:{ProductionDictionary[item.Key].ProductionsList[j].ProductionC.unitProductions[0].BuildingUID}生产值+0.05，进度：{ProductionDictionary[item.Key].ProductionsList[j].ProductionC.unitProductions[0].Schedule}");
                        }



                        if (ProductionDictionary[item.Key].ProductionsList[j].ProductionC.unitProductions[0].Schedule >= ProductionDictionary[item.Key].ProductionsList[j].ProductionC.unitProductions[0].TotalTime)
                        {
                            ProductionDictionary[item.Key].ProductionsList[j].ProductionC.unitProductions[0].IsProduction = true;
                        }

                    }

                    if (ProductionDictionary[item.Key].ProductionsList[j].ProductionC.unitProductions[0].IsProduction)
                    {
                        // 单位生产完成
                        //Debug.Log($"单位:{ProductionList[i].ProductionsList[j].ProductionC.unitProductions[0].BuildingUID}生产完成");

                        if (ProductionDictionary[item.Key].ProductionsList[j].ProductionC.unitProductions[0].Total > 1)
                        {
                            Debug.Log($"单位:{ProductionDictionary[item.Key].ProductionsList[j].ProductionC.unitProductions[0].BuildingUID}从列表里-1");
                            ProductionDictionary[item.Key].ProductionsList[j].ProductionC.unitProductions[0].Schedule = 0f;
                            ProductionDictionary[item.Key].ProductionsList[j].ProductionC.unitProductions[0].IsProduction = false;
                            ProductionDictionary[item.Key].ProductionsList[j].ProductionC.unitProductions[0].Total -= 1;
                        }
                        else
                        {
                            Debug.Log($"单位:{ProductionDictionary[item.Key].ProductionsList[j].ProductionC.unitProductions[0].BuildingUID}从列表里移除");
                            ProductionDictionary[item.Key].ProductionsList[j].ProductionC.unitProductions.RemoveAt(0);
                        }
                    }

                }

                // 装甲单位
                //if ()
                //{

                //}



            }
        }
    }
    
}


public class PlayerGroup
{
    public List<ProductionAgreement> ProductionsList { get; set; } = new List<ProductionAgreement>();
    public int PlayerUID { get; set; }
}

public class ProductionAgreement
{
    public AProduction ProductionA { get; set; } = new AProduction();
    public BProduction ProductionB { get; set; } = new BProduction();
    public CProduction ProductionC { get; set; } = new CProduction();
    public DProduction ProductionD { get; set; } = new DProduction();
}

public class AProduction // 主要建筑栏位
{
    public UnitProduction Building { get; set; } = null;// 当前生产的建筑单位
    public bool IsProduction { get; set; } = false;// 该生产栏位是否正在使用
    public bool IsSuspend { get; set; } = false;// 暂停生产

    public List<string> AvailableList { get; set; } = new List<string>(); // 可用生产列表
}

public class BProduction // 防御建筑栏位
{
    public UnitProduction Building { get; set; } = null;// 当前生产的建筑单位
    public bool IsProduction { get; set; } = false;// 该生产栏位是否正在使用
    public bool IsSuspend { get; set; } = false;// 暂停生产
    //public List<UnitProduction> unitProductions { get; set; } // 顺序生产列队

    public List<string> AvailableList { get; set; } = new List<string>(); // 可用生产列表
}

public class CProduction // 步兵单位栏位
{
    public bool IsSuspend { get; set; } // 暂停生产
    public List<UnitProduction> unitProductions { get; set; } = new List<UnitProduction>();// 顺序生产列队

    public List<string> AvailableList { get; set; } = new List<string>(); // 可用生产列表
}

public class DProduction // 装甲单位栏位
{
    public List<UnitProduction> unitProductionsA { get; set; } = new List<UnitProduction>();// 顺序生产列队(陆军/直升机)
    public bool IsSuspendA { get; set; } // 暂停生产
    public List<UnitProduction> unitProductionsB { get; set; } = new List<UnitProduction>();// 顺序生产列队(大型陆军单位)
    public bool IsSuspendB { get; set; }
    public List<UnitProduction> unitProductionsC { get; set; } = new List<UnitProduction>();// 顺序生产列队(战机)
    public bool IsSuspendC { get; set; }
    public List<UnitProduction> unitProductionsD { get; set; } = new List<UnitProduction>();// 顺序生产列队(海军)
    public bool IsSuspendD { get; set; }

    public List<string> AvailableList { get; set; } = new List<string>(); // 可用生产列表
}

public class UnitProduction // 生产单位
{
    public string BuildingUID { get; set; } // 单位uid
    public float Schedule { get; set; } = 0;// 当前生产的进度
    public float TotalTime { get; set; } // 需要的总进度
    public bool IsProduction { get; set; } // 是否完成
    public int Total {  get; set; } // 生产列队数量
}
