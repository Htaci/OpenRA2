using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class 设置按钮 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image Image;
    private bool isSwitch; // 开关
    public Color Color;

    public void Start()
    {
        Image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // 当鼠标进入开关时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        isSwitch = true;
        Debug.Log("进入1");
    }

    // 当鼠标离开按钮时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        isSwitch = false;
        Debug.Log("进入2");
    }

    // 当按下鼠标或触摸屏幕时触发
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("进入设置界面2");
    }

    // 当抬起鼠标或触摸屏幕时触发
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isSwitch)
        {
            Debug.Log("进入设置界面");
        }
        Debug.Log("进入设置界面");
    }
}
