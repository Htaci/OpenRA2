using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class 确定按钮 : MonoBehaviour,IPointerClickHandler
{
    // 地图名称
    public TMP_InputField inputMapName;
    // 地图宽度
    public TextMeshProUGUI inputMapWidth;
    // 地图高度
    public TextMeshProUGUI inputMapHeight;
    public void OnPointerClick(PointerEventData eventData)
    {
        // 检查地图名是否为空
        if (inputMapName.text == "")
        {
            // 为空，使用默认地图名+当前时间
            string mapName = "New Map";
        }
    }
}
