using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class 鼠标停留预览 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject 介绍面板;
    public Image Image;
    public TextMeshProUGUI 单位名;
    public TextMeshProUGUI 单位属性;
    public TextMeshProUGUI 单位介绍;

    public string 单位名string;
    public string 类型;
    public string 价格;
    public string 耗时;
    public string 护甲;
    public string 单位介绍string;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("鼠标进入按钮");
        单位名.text = $"{单位名string}";
        单位属性.text = $"类型：{类型}\r\n价格：{价格}\r\n生产耗时：{耗时}s\r\n护甲：{护甲}\r\n----------------------------------";
        单位介绍.text = $"{单位介绍string}";
        介绍面板.SetActive(true);
    }

    // 当鼠标离开按钮时调用的方法
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("鼠标离开按钮");
        介绍面板.SetActive(false);
    }
}
