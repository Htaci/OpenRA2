using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

//using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using Image = UnityEngine.UI.Image;


public class IconObjects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string unitName; // 单位名称（ID名称）
    public string IconName; // 图标名称（ID名称）

    public TextMeshProUGUI 加载百分比;
    public Image image; // 生产图标

    public Graphic graphic;

    public bool MouseClick = true;    // 鼠标是否可点击
    // 当鼠标进入时
    public void OnPointerEnter(PointerEventData eventData)
    {
        //MouseClick = true;
    }

    // 当鼠标离开时
    public void OnPointerExit(PointerEventData eventData)
    {
        //MouseClick = false;
    }
    // 图标被点击时
    public void OnPointerClick(PointerEventData eventData)
    {
        // 图标可点击时
        if (MouseClick)
        {
            // 获得当前图标单位的属性
            S_UnitUID unit = ResourcesCacheManager.Instance.unitUID[unitName];

            List<UnitProduction> List = null;

            switch (IconListManager.Instance.OpenPlane)
            {
                case 1:
                    // 主建筑面板

                    // 判断点击的按键类型
                    // 左键点击
                    if (eventData.button == PointerEventData.InputButton.Left)
                    {
                        //Debug.Log($"IconObjects: 对象 {unitName} 被点击，类型为左键");
                        // 判断生产位是否为正在使用的
                        if (ProductionManager.Instance.Production[IconListManager.Instance.PlayerUID].ProductionsList[IconListManager.Instance.OpenList].ProductionA.IsProduction)
                        {
                            //Debug.Log("IconObjects:已有建造");
                            // 如果建造完成
                            if (ProductionManager.Instance.Production[IconListManager.Instance.PlayerUID].ProductionsList[IconListManager.Instance.OpenList].ProductionA.Building.IsProduction)
                            {
                                Debug.Log($"IconObjects:建造对象( {ProductionManager.Instance.Production[IconListManager.Instance.PlayerUID].ProductionsList[IconListManager.Instance.OpenList].ProductionA.Building.BuildingUID} )建造完成，左键触发放置建筑");
                                // 开始放置建筑
                                建筑物放置 建筑物生成 = FindObjectOfType<建筑物放置>();
                                建筑物生成.放置预览建筑("");
                            }// 如果没有建造完成
                            else
                            {
                                Debug.Log("IconObjects:已有建造暂停，左键触发继续建造");
                                // 正在建造，继续建造
                                ProductionManager.Instance.Production[IconListManager.Instance.PlayerUID].ProductionsList[IconListManager.Instance.OpenList].ProductionA.IsSuspend = true;

                            }

                            
                        }
                        else
                        {
                            Debug.Log("IconObjects:无已有建造，新建建造对象：" + unitName);
                            // 不在建造，开始新建造
                            ProductionManager.Instance.Production[IconListManager.Instance.PlayerUID].ProductionsList[IconListManager.Instance.OpenList].ProductionA.Building =
                                new UnitProduction
                                {
                                    BuildingUID = unitName,
                                    Schedule = 0,
                                    TotalTime = unit.TotalTime,
                                    Total = 1
                                };
                            ProductionManager.Instance.Production[IconListManager.Instance.PlayerUID].ProductionsList[IconListManager.Instance.OpenList].ProductionA.IsSuspend = true;
                            ProductionManager.Instance.Production[IconListManager.Instance.PlayerUID].ProductionsList[IconListManager.Instance.OpenList].ProductionA.IsProduction = true;

                        }

                        // 更新一次列表状态
                        IconListManager.Instance.isIconlistUpdate = true;
                        IconListManager.Instance.isIconListUpdateOften = true;
                    }
                    // 右键点击
                    else if (eventData.button == PointerEventData.InputButton.Right)
                    {
                        //Debug.Log($"IconObjects: 对象 {unitName} 被点击，类型为右键");
                        // 判断生产位是否为正在使用的
                        if (ProductionManager.Instance.Production[IconListManager.Instance.PlayerUID].ProductionsList[IconListManager.Instance.OpenList].ProductionA.IsProduction)
                        {
                            // 判断是否是非暂停状态
                            if (ProductionManager.Instance.Production[IconListManager.Instance.PlayerUID].ProductionsList[IconListManager.Instance.OpenList].ProductionA.IsSuspend)
                            {
                                Debug.Log("IconObjects:非暂停状态，右键暂停建造");
                                // 设置为暂停
                                ProductionManager.Instance.Production[IconListManager.Instance.PlayerUID].ProductionsList[IconListManager.Instance.OpenList].ProductionA.IsSuspend = false;
                            }
                            // 已经是暂停状态
                            else
                            {
                                Debug.Log("IconObjects:已是暂停状态，右键取消建造");
                                // 清除生产列表
                                ProductionManager.Instance.Production[IconListManager.Instance.PlayerUID].ProductionsList[IconListManager.Instance.OpenList].ProductionA.Building = null;
                                // 取消正在使用状态
                                ProductionManager.Instance.Production[IconListManager.Instance.PlayerUID].ProductionsList[IconListManager.Instance.OpenList].ProductionA.IsProduction = false;
                            }
                            // 更新一次列表状态
                            IconListManager.Instance.isIconlistUpdate = true;
                            IconListManager.Instance.isIconListUpdateOften = true;
                        }
                        else
                        {
                            Debug.LogWarning("IconObjects:该对象暂无使用，右键不生效");
                        }

                    }
                    // 主建筑列表没有中键功能

                    break;
                case 2:
                    // 防御建筑面板
                    //List = ProductionManager.Instance.Production[IconListManager.Instance.PlayerUID].ProductionsList[IconListManager.Instance.OpenList].ProductionB.Building;
                    break;
                case 3:
                    // 步兵单位面板
                    //List = ProductionManager.Instance.Production[IconListManager.Instance.PlayerUID].ProductionsList[IconListManager.Instance.OpenList].ProductionC.Building;
                    break;
                case 4:
                    // 装甲单位面板
                    //List = ProductionManager.Instance.Production[IconListManager.Instance.PlayerUID].ProductionsList[IconListManager.Instance.OpenList].ProductionD;
                    break;
                default:
                    Console.WriteLine("无效值");
                    break;
            }

            // 判断点击的按键类型
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                //Debug.Log("左键被点击");
                IconListManager.Instance.Icon_Click(unitName,1);

            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                //Debug.Log("右键被点击");
                IconListManager.Instance.Icon_Click(unitName, 2);
            }
            else if (eventData.button == PointerEventData.InputButton.Middle)
            {
                //Debug.Log("中键被点击");
                IconListManager.Instance.Icon_Click(unitName, 3);
            }
        }
    }


    // 初始化完成后执行 
    public void Initialize()
    {
        // 获取子对象在其父对象中的索引值
        int index = gameObject.transform.GetSiblingIndex();

        // 输出索引值（测试）
        //Debug.Log($"子对象 {unitName} 的索引值是 {index}");

        graphic.CrossFadeAlpha(0f, 0f, true);
    }

    // Update is called once per frame
    //void FixedUpdate()
    //{

    //    加载百分比.text = "";
    //}
    //void Update()
    //{
    
    //}


    public void IconClick()
    {

    }
    // 被激活时
    void OnEnable()
    {
        int index = gameObject.transform.GetSiblingIndex();
        // 然后在下一帧调用CrossFadeAlpha，将透明度平滑过渡到1
        Debug.Log($"图标对象（ {unitName} ）的 FadeIn 在 {index*0.05f} 秒后触发");
        Invoke("FadeIn", index * 0.05f);
    }

    // 图标淡出效果
    private void FadeIn()
    {
        // 将透明度在0.2秒内平滑过渡到1，忽略时间缩放
        graphic.CrossFadeAlpha(1f, 0.2f, true);
        //Debug.Log($"子对象 {unitName} 的 FadeIn 被触发");
    }
    // 被禁用时
    void OnDisable()
    {
        // 更改透明度为 完全透明
        graphic.CrossFadeAlpha(0f, 0f, true);
    }
}
