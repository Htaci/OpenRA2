using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SubPanelNavigation : MonoBehaviour
{
    public Toggle[] smallCategoryToggles; // 存储所有小分类Toggles的数组

    void Start()
    {
        // 获取这个GameObject上的Toggle组件
        Toggle toggle = GetComponent<Toggle>();
        // 添加Toggle值改变时的事件监听器
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(OnMajorCategoryChanged);
            //Debug.Log("Major category toggle listener added.");
        }
        else
        {
            //Debug.Log("Toggle component not found on major category button.");
        }
    }

    void OnMajorCategoryChanged(bool isOn)
    {
        if (isOn)
        {
            //Debug.Log("Major category changed.");
            // 激活小分类中的第一个Toggle
            if (smallCategoryToggles.Length > 0)
            {
                smallCategoryToggles[0].isOn = true;
                // 确保只有第一个Toggle是激活状态
                for (int i = 1; i < smallCategoryToggles.Length; i++)
                {
                    smallCategoryToggles[i].isOn = false;
                }
            }
        }
    }
}
