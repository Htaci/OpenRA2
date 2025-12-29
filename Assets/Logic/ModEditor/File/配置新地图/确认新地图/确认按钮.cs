using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class 确认地图配置_确认_按钮 : MonoBehaviour, IPointerClickHandler
{
    public GameObject 新建地图面板;

    public int MapWidth;
    public int MapHeight;

    public 地图编辑器工具 地图编辑器工具;

    // 被点击后
    public void OnPointerClick(PointerEventData eventData)
    {
        //地图编辑器工具.NewMap("新地图", MapWidth, MapHeight);

        新建地图面板.SetActive(false);
    }
}
