using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// 打开新建项目的面板
/// </summary>
public class 新建项目按钮 : MonoBehaviour, IPointerClickHandler
{
    // 新建项目的面板
    public GameObject 新建项目面板;

    // 被点击时
    public void OnPointerClick(PointerEventData eventData)
    {
        新建项目面板.SetActive(true);
    }
}
