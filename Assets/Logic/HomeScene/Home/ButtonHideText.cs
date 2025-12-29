using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

// 在按钮按下时隐藏按钮的文本，松开时显示
public class ButtonHideText : MonoBehaviour
{
    public Button button; // 按钮
    public TMP_Text buttonText; // 按钮上的文本组件

    void Start()
    {
        // 为按钮的按下和松开事件添加监听
        button.onClick.AddListener(OnButtonClick);

        // 获取按钮的 EventTrigger 组件，如果没有则添加一个
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // 添加按下事件监听
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => { OnPointerDown(); });
        trigger.triggers.Add(pointerDownEntry);

        // 添加松开事件监听
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) => { OnPointerUp(); });
        trigger.triggers.Add(pointerUpEntry);
    }

    private void OnButtonClick()
    {
        // 按钮点击时的操作
    }

    private void OnPointerDown()
    {
        // 按钮按下时隐藏文本
        buttonText.gameObject.SetActive(false);
    }

    private void OnPointerUp()
    {
        // 按钮松开时显示文本
        buttonText.gameObject.SetActive(true);
    }
}
