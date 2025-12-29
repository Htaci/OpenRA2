using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using Slider = UnityEngine.UI.Slider;

public class IconListManager : MonoBehaviour
{
    // 单例模式
    public static IconListManager Instance { get; private set; }

    public GameObject parentObject;

    public int PlayerUID =114; // 玩家id
    public int OpenList=0; // 玩家当前所在的协议组
    public int OpenPlane; // 当前打开的面板，
    // 1：主要建筑
    // 2：防御建筑
    // 3：步兵单位
    // 4：装甲单位

    public GameObject gameIcon; // 图标预制件
    public List<IconObjects> iconObjects = new List<IconObjects>(); // 图标对象池
    public List<IconObjects> openObjects = new List<IconObjects>(); // 启用的图标
    public List<IconObjects> inuseObjects = new List<IconObjects>(); // 正在使用的图标，用于显示
    public bool isIconlistUpdate = false; // 图标列表是否需要更新（更新整个openObjects列表的状态）
    public bool isIconListUpdateOften = true; // 图标列表是否需要更新（每帧更新）


    public Slider progressBar;  // 生产进度条

    //public ProductionManager productionManager; // 建造管理器

    // Start is called before the first frame update
    void Start()
    {
        // 获取当前脚本所在的游戏对象
        parentObject = gameObject;

        // 构造测试数据
        S_UnitUID a = new S_UnitUID
        {
            Name = "bingying",
            TotalTime = 12,
            Type = "Building",

        };
        S_UnitUID b = new S_UnitUID
        {
            Name = "fadiancang",
            TotalTime = 12,
            Type = "Building",

        };
        S_UnitUID c = new S_UnitUID
        {
            Name = "kuangsi",
            TotalTime = 12,
            Type = "Building",

        };
        // 添加缓存数据，用于查找单位属性（测试）
        ResourcesCacheManager.Instance.unitUID.TryAdd("bingying",a);
        ResourcesCacheManager.Instance.unitUID.TryAdd("fadiancang", b);
        ResourcesCacheManager.Instance.unitUID.TryAdd("kuangsi", c);
        // 新建一个玩家阵营
        ProductionManager.Instance.Production.TryAdd(PlayerUID, new PlayerGroup { });
        Debug.Log("建筑管理器：新建了一个阵营 " + IconListManager.Instance.PlayerUID);
        //// 新建一个协议组
        ProductionManager.Instance.Production[PlayerUID].ProductionsList.Add(new ProductionAgreement { });
        //Debug.Log("建筑管理器.ProductionList.ProductionsList列表长度：" + productionManager.ProductionList[0].ProductionsList.Count);
        // 在协议组里添加建造对象（测试）
        ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionA.AvailableList.Add("bingying");
        ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionA.AvailableList.Add("fadiancang");
        ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionA.AvailableList.Add("kuangsi");
        Debug.Log($"单位:{ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionA.AvailableList[0]}被添加到列表里");
        if (ProductionManager.Instance.Production[PlayerUID] == null)
        {
            Debug.LogError("新增玩家失败");
        }
        else if (ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionA.AvailableList.Count == 0)
        {
            Debug.LogError("新增单位失败");
        }


        //Close_All();
        //Open_IconPanel(1);
    }

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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (OpenPlane)
        {
            case 1:
                // 主建筑面板
                // 判断这个列表是否是正在使用的状态
                if (ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionA.IsProduction)
                {
                    // 是否 是需要更新
                    if (isIconlistUpdate)
                    {
                        // 更新图标显示：颜色，是否可点击，正在使用的图标
                        Debug.Log("ProductionA.IsProduction的使用状态是" + ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionA.IsProduction);
                        foreach (var a in openObjects)
                        {
                            // 判断当前图标是否是正在建造的对象
                            if (a.unitName == ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionA.Building.BuildingUID)
                            {
                                // 是正在建造的对象
                                a.image.color = new Color32(255, 255, 255, 255); // 图标正常
                                a.MouseClick = true; // 可点击
                                // 检测是否 是暂停
                                if (!ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionA.IsSuspend)
                                {
                                    isIconListUpdateOften = false;
                                    a.加载百分比.text = "暂停";
                                }

                                // 检测是否 是已完成状态
                                if (ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionA.Building.IsProduction)
                                {
                                    isIconListUpdateOften = false;
                                    a.加载百分比.text = "就绪";
                                }

                                // 判断当前正在使用的是否只有一个，因为是主建筑列表，当前正在使用的图标只能为一个
                                if (inuseObjects.Count != 0)
                                {
                                    inuseObjects.Clear();
                                    inuseObjects.Add(a); // 添加到正在使用的图标
                                }
                                else
                                {
                                    inuseObjects.Add(a); // 添加到正在使用的图标
                                }
                            }
                            else
                            {
                                // 不是正在建造的对象
                                a.image.color = new Color32(150, 150, 150, 255); // 图标变灰
                                a.MouseClick = false; // 不可点击
                                a.加载百分比.text = null;
                            }

                        }

                    }

                    // 每帧更新
                    if (isIconListUpdateOften)
                    {
                        // 当有正在使用的图标时
                        if (inuseObjects.Count != 0)
                        {
                            //Debug.LogWarning("图标管理器:列表内有当前正在使用的图标");Schedule TotalTime
                            //float currentTime = Time.time; // 当前时间
                            //float t = (currentTime - ProductionManager.Instance.lastFixedUpdateTime) / (1f / 20f); // 计算插值比例
                            //t = Mathf.Clamp01(t); // 确保 t 在 0 到 1 之间

                            // 使用 Mathf.Lerp 进行线性插值
                            //float interpolatedSchedule = Mathf.Lerp(lastSchedule, currentSchedule, t);

                            // 计算百分比
                            //float percentage = interpolatedSchedule / ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionA.Building.TotalTime;
                            float percentage = ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionA.Building.Schedule / ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionA.Building.TotalTime *100;
                            progressBar.value = percentage/100;
                            inuseObjects[0].加载百分比.text = $"{percentage.ToString("f1")}%";
                        }
                    }
                }
                else
                {
                    // 这个列表没有在使用
                    if (isIconlistUpdate)
                    {
                        inuseObjects.Clear();
                        foreach (var a in openObjects)
                        {
                            // 是正在建造的对象
                            a.image.color = new Color32(255, 255, 255, 255); // 图标正常
                            a.MouseClick = true; // 可点击
                            a.加载百分比.text = null;
                        }
                    }

                }




                // 更新一次后，设置为不需要更新
                isIconlistUpdate = false;
        
        break;
            case 2:
                // 防御建筑面板
                //List = ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionB.AvailableList;
                break;
            case 3:
                // 步兵单位面板
                //List = ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionC.AvailableList;
                break;
            case 4:
                // 装甲单位面板
                //List = ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionD.AvailableList;
                break;
            default:
                //Console.WriteLine("无打开的面板");
                break;
        }
        
    }

    public void Open_IconPanel(int Panelid)
    {
        isIconListUpdateOften = true;
        isIconlistUpdate = true;
        List<string> List = null;

        switch (Panelid)
        {
            case 1:
                // 主建筑面板
                List = ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionA.AvailableList;
                break;
            case 2:
                // 防御建筑面板
                List = ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionB.AvailableList;
                break;
            case 3:
                // 步兵单位面板
                List = ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionC.AvailableList;
                break;
            case 4:
                // 装甲单位面板
                List = ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionD.AvailableList;
                break;
            default:
                Console.WriteLine("无效值");
                break;
        }
        OpenPlane = Panelid;
        if (List != null)
        {
            foreach (var a in List)
            {
                S_UnitUID unitUID = ResourcesCacheManager.Instance.unitUID[a];
                Debug.Log("当前处理的图标：" + a);
                bool d = false;
                // 检查图标池是否有这个图标
                foreach (var b in iconObjects)
                {
                    // 如果图标池里已经有这个图标了
                    if (b.unitName == a)
                    {
                        openObjects.Add(b);
                        Debug.Log("图标池有当前处理的图标：" + a);
                        d = true;
                        GameObject childGameObject = b.gameObject;
                        childGameObject.SetActive(true);
                    }

                }

                if (!d) // 没有这个图标，新建一个
                {
                    // 实例化预制体并设置为当前对象的子对象
                    GameObject gameIconInstance = Instantiate(gameIcon);
                    gameIconInstance.transform.SetParent(this.transform);
                    // 新建的图标
                    GameObject instance = gameIconInstance;

                    // 新建图标的 IconObjects 脚本
                    IconObjects c = instance.GetComponent<IconObjects>();

                    c.unitName = a; // 设置单位id
                    Debug.Log($"新建图标（{a}）初始化完成");
                    c.Initialize(); // 初始化方法

                    instance.SetActive(true) ; // 设置为激活状态
                    openObjects.Add(c); // 添加到 当前打开的图标列表
                    iconObjects.Add(c); // 添加到 图标池列表
                    Debug.Log($"新建图标（{a}）的操作完成");
                }
            }
        }



    }


    public void Close_All()
    {
        // 遍历当前游戏对象及其所有子对象
        foreach (Transform child in parentObject.transform)
        {
            // 检查子对象是否带有 IconObjects 脚本
            IconObjects iconObject = child.GetComponent<IconObjects>();
            if (iconObject != null)
            {
                // 获取子对象所属的游戏对象
                GameObject childGameObject = child.gameObject;
                childGameObject.SetActive(false);
            }
        }
        openObjects.Clear(); // 清除打开的图标列表
    }


    public void Icon_Click(string id,int type)
    {
        switch (OpenPlane)
        {
            case 1:
                // 主要建筑
                if (type == 1)
                {
                    // 左键点击
                    if (!ProductionManager.Instance.Production[PlayerUID].ProductionsList[OpenList].ProductionA.IsProduction)
                    {
                        // 建造列表没有被占用时执行

                    }
                }
                else if (type == 2)
                {

                }
                else if(type == 3)
                {

                }
                break;
            case 2:
                // 防御建筑

                break;
            case 3:
                // 步兵单位

                break;
            case 4:
                // 装甲单位

                break;
            default:
                Console.WriteLine("无效值");
                break;
        }
    }
}
