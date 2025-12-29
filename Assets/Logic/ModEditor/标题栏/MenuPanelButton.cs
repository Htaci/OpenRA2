using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuPanelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image image;
    // 调用方法目标类
    public object target;
    
    void Awake()
    {
        // 判断此对象上有没有 Image 组件
        if (GetComponent<Image>() != null)
        {
            // 获取Image组件
            image = GetComponent<Image>();
        }
        else
        {
            // 如果没有，则创建一个Image组件
            image = gameObject.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0);
        }
    }
    // 点击时
    public virtual void OnPointerClick(PointerEventData eventData)
    {

    }

    // 当鼠标进入时
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (image != null)
            image.color = new Color(1, 1, 1, 0.1f);
        MenuPanel.isShow = true;
    }

    // 当鼠标离开时
    public void OnPointerExit(PointerEventData eventData)
    {
        if (image != null)
            image.color = new Color(0, 0, 0, 0);
        MenuPanel.isShow = false;
    }
}
