using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class 建造栏开关 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image Image;
    public bool isSwitch; // 开关
    public Color Color;
    public List<建造栏开关> ProductionToggleList = new List<建造栏开关>();
    public int A;

    public void Start()
    {
        Image = GetComponent<Image>();
    }
    // 当鼠标进入开关时触发
    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    // 当鼠标离开按钮时触发
    public void OnPointerExit(PointerEventData eventData)
    {

    }

    // 当按下鼠标或触摸屏幕时触发
    public void OnPointerDown(PointerEventData eventData)
    {

    }

    // 当抬起鼠标或触摸屏幕时触发
    public void OnPointerUp(PointerEventData eventData)
    {

    }

    // 被点击时
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"生产栏{A}被点击");
        IconListManager.Instance.Close_All();
        IconListManager.Instance.Open_IconPanel(A);

        foreach (var item in ProductionToggleList)
        {
            item.isSwitch = false;
        }

        isSwitch = true;
    }

}
