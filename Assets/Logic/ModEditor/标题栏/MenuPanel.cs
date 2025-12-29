using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 菜单容器，当菜单按钮被点击时，显示菜单
/// </summary>
public class MenuPanel : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    public static List<GameObject> list = new List<GameObject>();
    public static bool isShow = false;

    void Awake()
    {
        list.Add(gameObject);
        // 检查 Image 是否为空，为空则添加
        if (GetComponent<Image>() == null)
        {
            Image image = gameObject.AddComponent<Image>();
            // 设置颜色为 2C2C2C 
            image.color = new Color32(44, 44, 44, 255);
        }
    }
    // 当鼠标进入时
    public void OnPointerEnter(PointerEventData eventData)
    {
        isShow = true;
    }

    // 当鼠标离开时
    public void OnPointerExit(PointerEventData eventData)
    {
        isShow = false;
    }
}
