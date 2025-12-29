using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class 新建地图 : MonoBehaviour, IPointerClickHandler
{
    public GameObject 新建地图面板;
    // 被点击时
    public void OnPointerClick(PointerEventData eventData)
    {
        新建地图面板.SetActive(true);
    }
}
