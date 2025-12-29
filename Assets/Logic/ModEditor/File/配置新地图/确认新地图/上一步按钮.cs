using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class 确认地图配置_上一步_按钮 : MonoBehaviour, IPointerClickHandler
{
    public GameObject 确认面板;

    public GameObject 配置面板;
    // 被点击后
    public void OnPointerClick(PointerEventData eventData)
    {
        确认面板.SetActive(false);
        配置面板.SetActive(true);
    }
}
