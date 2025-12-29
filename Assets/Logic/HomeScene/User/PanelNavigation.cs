using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelNavigation: MonoBehaviour
{
    [System.Serializable]
    public struct TogglePanelPair
    {
        public Toggle toggle;
        public GameObject panel;
    }

    public List<TogglePanelPair> togglePanelPairs;

    void Start()
    {
        // 遍历所有的Toggle和Panel对
        foreach (var pair in togglePanelPairs)
        {
            // 确保初始状态与Toggle的状态一致
            pair.panel.SetActive(pair.toggle.isOn);

            // 监听每个Toggle的状态变化
            pair.toggle.onValueChanged.AddListener((isOn) => OnToggleValueChanged(pair.toggle, isOn));
        }
    }

    // 当Toggle的值改变时调用的方法
    private void OnToggleValueChanged(Toggle changedToggle, bool isOn)
    {
        // 查找对应的面板并更新其状态
        foreach (var pair in togglePanelPairs)
        {
            if (pair.toggle == changedToggle)
            {
                pair.panel.SetActive(isOn);
                break;
            }
        }
    }

    void OnDestroy()
    {
        // 移除所有监听器以避免内存泄漏
        foreach (var pair in togglePanelPairs)
        {
            pair.toggle.onValueChanged.RemoveAllListeners();
        }
    }
}
