using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class 标题栏下拉按钮 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 展开的下拉面板
    public GameObject GameObject;

    // 需要触发的任务
    public 任务 任务1;

    public bool isShow = false;
    // Start is called before the first frame update
    void Start()
    {
        标题栏按钮管理器.Instance.按钮列表.Add(this);
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

    public void 触发任务()
    {
        switch (任务1)
        {
            case 任务.打开面板:
                GameObject.SetActive(true);
                break;
            
        }
    }
}

public enum 任务
{
    打开面板,
    新建项目,
}