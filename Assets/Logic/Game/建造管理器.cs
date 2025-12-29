using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ProductionManager : MonoBehaviour
{

    // 单例模式
    public static ProductionManager Instance { get; private set; }

    public Dictionary<int,PlayerGroup> Production = new Dictionary<int, PlayerGroup>();

    public float lastFixedUpdateTime = 0f; // 记录更新时间

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保在场景切换时不被销毁
        }
        else
        {
            Destroy(gameObject);
        }
        //新建C生产任务();

        // 新建一个玩家阵营
        //Instance.Production.TryAdd(IconListManager.Instance.PlayerUID, new PlayerGroup { });
        //Debug.Log("建筑管理器：新建了一个阵营 " + IconListManager.Instance.PlayerUID);
        // 新建一个协议组
        //Production[IconListManager.Instance.PlayerUID].ProductionsList.Add(new ProductionAgreement { });
        //Debug.Log("建筑管理器.ProductionList.ProductionsList列表长度：" + productionManager.ProductionList[0].ProductionsList.Count);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log($"建筑更新管理器");
        // PlayerGroup 层
        foreach (var item in Production) 
        {
            // ProductionAgreement 层
            for (int j = 0; j < Production[item.Key].ProductionsList.Count; j++)
            {
                //Debug.Log($"建筑更新管理器：循环 i 为:{i} ，j 为:{j}" );
                // 主要建筑生产更新
                if (Production[item.Key].ProductionsList[j].ProductionA.Building != null) // 如果当前正在生产的单位内容不为空
                {
                    //Debug.LogWarning($"建造管理器:{item.Key}的第{j}组的主要建筑不为空");
                    // 判断是否 是已经完成生产了
                    if (!Production[item.Key].ProductionsList[j].ProductionA.Building.IsProduction)
                    {
                        // 判断是否没有被暂停
                        if (Production[item.Key].ProductionsList[j].ProductionA.IsSuspend)
                        {
                            // 设置该栏被占用
                            //Production[item.Key].ProductionsList[j].ProductionA.IsProduction = true;
                            // 生产进度增加
                            Production[item.Key].ProductionsList[j].ProductionA.Building.Schedule += 0.05f;
                            //Debug.Log($"建造管理器:主要建筑 {Production[item.Key].ProductionsList[j].ProductionA.Building.BuildingUID} 生产进度+0.05，总进度:{Production[item.Key].ProductionsList[j].ProductionA.Building.Schedule}");
                        }
                        else
                        {
                            Debug.LogWarning($"建造管理器:{item.Key}的第{j}组的主要建筑被暂停了");
                        }
                        // 如果生产完成
                        if (Production[item.Key].ProductionsList[j].ProductionA.Building.Schedule >= Production[item.Key].ProductionsList[j].ProductionA.Building.TotalTime)
                        {
                            Debug.Log($"建造管理器:主要建筑 {Production[item.Key].ProductionsList[j].ProductionA.Building.BuildingUID} 生产完成，总进度:{Production[item.Key].ProductionsList[j].ProductionA.Building.Schedule}");
                            // 暂停生产
                            Production[item.Key].ProductionsList[j].ProductionA.IsSuspend = false;
                            // 设置生产完成
                            Production[item.Key].ProductionsList[j].ProductionA.Building.IsProduction = true;
                            IconListManager.Instance.isIconlistUpdate = true;
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
                if (Production[item.Key].ProductionsList[j].ProductionB.Building != null) // 如果当前正在生产的单位内容不为空
                {
                    if (!Production[item.Key].ProductionsList[j].ProductionB.Building.IsProduction)
                    {
                        if (!Production[item.Key].ProductionsList[j].ProductionB.IsSuspend)
                        {
                            Production[item.Key].ProductionsList[j].ProductionB.IsProduction = true;
                            Production[item.Key].ProductionsList[j].ProductionB.Building.Schedule += 0.05f;
                        }
                        // 如果生产完成
                        if (Production[item.Key].ProductionsList[j].ProductionB.Building.Schedule >= Production[item.Key].ProductionsList[j].ProductionB.Building.TotalTime)
                        {
                            // 暂停生产
                            Production[item.Key].ProductionsList[j].ProductionB.IsSuspend = true;
                        }
                    }
                }
                else if (Production[item.Key].ProductionsList[j].ProductionB.Building == null) // 如果当前正在生产的单位内容为空
                {
                    // 取消占用栏
                    Production[item.Key].ProductionsList[j].ProductionB.IsProduction = false;
                }

                // 步兵单位生产更新
                if (Production[item.Key].ProductionsList[j].ProductionC.unitProductions.Count != 0)
                {
                    if (!Production[item.Key].ProductionsList[j].ProductionC.unitProductions[0].IsProduction)
                    {
                        if (!Production[item.Key].ProductionsList[j].ProductionC.IsSuspend)
                        {
                            
                            Production[item.Key].ProductionsList[j].ProductionC.unitProductions[0].Schedule += 0.05f;
                            Debug.Log($"单位:{Production[item.Key].ProductionsList[j].ProductionC.unitProductions[0].BuildingUID}生产值+0.05，进度：{Production[item.Key].ProductionsList[j].ProductionC.unitProductions[0].Schedule}");
                        }



                        if (Production[item.Key].ProductionsList[j].ProductionC.unitProductions[0].Schedule >= Production[item.Key].ProductionsList[j].ProductionC.unitProductions[0].TotalTime)
                        {
                            Production[item.Key].ProductionsList[j].ProductionC.unitProductions[0].IsProduction = true;
                        }

                    }

                    if (Production[item.Key].ProductionsList[j].ProductionC.unitProductions[0].IsProduction)
                    {
                        // 单位生产完成
                        //Debug.Log($"单位:{ProductionList[i].ProductionsList[j].ProductionC.unitProductions[0].BuildingUID}生产完成");

                        if (Production[item.Key].ProductionsList[j].ProductionC.unitProductions[0].Total > 1)
                        {
                            Debug.Log($"单位:{Production[item.Key].ProductionsList[j].ProductionC.unitProductions[0].BuildingUID}从列表里-1");
                            Production[item.Key].ProductionsList[j].ProductionC.unitProductions[0].Schedule = 0f;
                            Production[item.Key].ProductionsList[j].ProductionC.unitProductions[0].IsProduction = false;
                            Production[item.Key].ProductionsList[j].ProductionC.unitProductions[0].Total -= 1;
                        }
                        else
                        {
                            Debug.Log($"单位:{Production[item.Key].ProductionsList[j].ProductionC.unitProductions[0].BuildingUID}从列表里移除");
                            Production[item.Key].ProductionsList[j].ProductionC.unitProductions.RemoveAt(0);
                        }
                    }

                }

                // 装甲单位
                //if ()
                //{

                //}



            }
        }

        lastFixedUpdateTime = Time.time; // 记录更新时间
    }


    public void 新建A生产任务()
    {

    }
    public void 新建B生产任务()
    {

    }
    public void 新建C生产任务()
    {
        // 新建一个玩家阵营
        Production.TryAdd(1000,new PlayerGroup {  });
        Debug.Log("建筑管理器.ProductionList列表长度：" + Production.Count);
        // 新建一个协议组
        Production[1000].ProductionsList.Add(new ProductionAgreement { });
        Debug.Log("建筑管理器.ProductionList.ProductionsList列表长度：" + Production[0].ProductionsList.Count);
        // 在协议组里添加建造对象
        Production[0].ProductionsList[0].ProductionC.unitProductions.Add(new UnitProduction { BuildingUID = "测试动员兵" , TotalTime = 6.0f, Total = 2 });
        //if (ProductionList[0].ProductionsList[0].ProductionC == null)
        //{
        //    Debug.Log("建筑管理器.ProductionList[0].ProductionsList[0].ProductionC为空");

        //}
        //else
        //{
        //    Debug.Log("建筑管理器.ProductionList[0].ProductionsList[0].ProductionC不为空");
        //}
        //Debug.Log("建筑管理器.ProductionList[0].ProductionsList[0].ProductionC.unitProductions列表长度：" + ProductionList[0].ProductionsList[0].ProductionC.unitProductions.Count);
        Debug.Log($"单位:{Production[0].ProductionsList[0].ProductionC.unitProductions[0].BuildingUID}被添加到列表里");
    }
    public void 新建D生产任务()
    {

    }
}



// public class PlayerGroup
// {
//     public List<ProductionAgreement> ProductionsList { get; set; } = new List<ProductionAgreement>();
//     public int PlayerUID { get; set; }
// }

// public class ProductionAgreement
// {
//     public AProduction ProductionA { get; set; } = new AProduction();
//     public BProduction ProductionB { get; set; } = new BProduction();
//     public CProduction ProductionC { get; set; } = new CProduction();
//     public DProduction ProductionD { get; set; } = new DProduction();
// }

// public class AProduction // 主要建筑栏位
// {
//     public UnitProduction Building { get; set; } = null;// 当前生产的建筑单位
//     public bool IsProduction { get; set; } = false;// 该生产栏位是否正在使用
//     public bool IsSuspend { get; set; } = false;// 暂停生产

//     public List<string> AvailableList { get; set; } = new List<string>(); // 可用生产列表
// }

// public class BProduction // 防御建筑栏位
// {
//     public UnitProduction Building { get; set; } = null;// 当前生产的建筑单位
//     public bool IsProduction { get; set; } = false;// 该生产栏位是否正在使用
//     public bool IsSuspend { get; set; } = false;// 暂停生产
//     //public List<UnitProduction> unitProductions { get; set; } // 顺序生产列队

//     public List<string> AvailableList { get; set; } = new List<string>(); // 可用生产列表
// }

// public class CProduction // 步兵单位栏位
// {
//     public bool IsSuspend { get; set; } // 暂停生产
//     public List<UnitProduction> unitProductions { get; set; } = new List<UnitProduction>();// 顺序生产列队

//     public List<string> AvailableList { get; set; } = new List<string>(); // 可用生产列表
// }

// public class DProduction // 装甲单位栏位
// {
//     public List<UnitProduction> unitProductionsA { get; set; } = new List<UnitProduction>();// 顺序生产列队(陆军/直升机)
//     public bool IsSuspendA { get; set; } // 暂停生产
//     public List<UnitProduction> unitProductionsB { get; set; } = new List<UnitProduction>();// 顺序生产列队(大型陆军单位)
//     public bool IsSuspendB { get; set; }
//     public List<UnitProduction> unitProductionsC { get; set; } = new List<UnitProduction>();// 顺序生产列队(战机)
//     public bool IsSuspendC { get; set; }
//     public List<UnitProduction> unitProductionsD { get; set; } = new List<UnitProduction>();// 顺序生产列队(海军)
//     public bool IsSuspendD { get; set; }

//     public List<string> AvailableList { get; set; } = new List<string>(); // 可用生产列表
// }

// public class UnitProduction // 生产单位
// {
//     public string BuildingUID { get; set; } // 单位uid
//     public float Schedule { get; set; } = 0;// 当前生产的进度
//     public float TotalTime { get; set; } // 需要的总进度
//     public bool IsProduction { get; set; } // 是否完成
//     public int Total {  get; set; } // 生产列队数量
// }