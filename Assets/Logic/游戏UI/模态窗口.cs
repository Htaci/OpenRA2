using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/模拟窗口", 11)]
public class SimulationWindow: MonoBehaviour
{
    // 颜色
    public Color color = Color.white;
    [Header("基础设置")]
    // 是否为模态窗口
    [Tooltip("是否为模态窗口")]
    public bool isModal = false;

    // 是否强制不允许用户关闭
    [Tooltip("是否显示模拟系统标题栏")]
    public bool forceNotClose = true;

    [Tooltip("是否允许通过标题栏拖动")]
    public bool allowDrag = true;

    [Header("高级设置")]

    // 圆角设置
    [Tooltip("圆角设置")]
    public float cornerRadius = 0;

    // 窗口是否有阴影
    [Tooltip("窗口是否有阴影")]
    public bool shadow = false;

    // 亚克力
    [Tooltip("亚克力")]
    public bool acrylic = false;

    // 亚克力透明度
    [Tooltip("亚克力透明度")]
    [Range(0, 1)]
    public float acrylicAlpha = 0.5f;

}
