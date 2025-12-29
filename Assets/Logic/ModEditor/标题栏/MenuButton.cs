using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 编辑器内的标题栏自定义菜单按钮，按下后展开菜单
/// </summary>
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // 展开的下拉面板
    public GameObject GameObject;
    // 按钮自身的图像
    public Image image;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if(GameObject != null)
        {
            // 判断是否已经是打开的状态
            if (GameObject.activeSelf)
            {
                GameObject.SetActive(false);
            }
            else
            {
                GameObject.SetActive(true);
            }
        }
        
        // 关闭所有带有脚本 下拉容器 的对象
        foreach (GameObject obj in MenuPanel.list)
        {
            if (obj != GameObject)
            {
                obj.SetActive(false);
            }
            
        }
    }
}

