using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[System.Serializable]
public class TopMenuItem
{
    public string title;
    public List<TopMenuSubItem> options;
    [HideInInspector]
    public GameObject TopMenuSubItemPanel;
}

// 子选项
[System.Serializable]
public class TopMenuSubItem
{
    public string title;
    public string action; // 可用于指定点击时触发的方法名称
    public List<TopMenuSubItem> options;
}

public class TopMenuList : MonoBehaviour
{
    public List<TopMenuItem> menuItems = new List<TopMenuItem>();
    // 记录哪个 TopMenuItem 的下拉菜单打开了
    private TopMenuItem activeMenuItem = null;
    // 记录当前鼠标停留在哪个 TopMenuItem 上
    private TopMenuItem hoveredMenuItem = null;
    // 鼠标是否在任何下拉菜单上
    private bool isPointerOverDropdown = false;

    // Start is called before the first frame update
    void Start()
    {
        // 在当前游戏对象上创建一个子对象用于存放菜单
        // 创建对象并设置为当前对象的子对象
        GameObject menuContainer = new GameObject("TopMenuContainer");
        // 设置父对象
        menuContainer.transform.SetParent(this.transform);
        // 设置锚点为左上角，轴心为x0y0.5，横向内容自动适配
        RectTransform rectTransform = menuContainer.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        // 设置轴心点为左上角
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(0, 0);
        // 设置顶部菜单栏的缩放
        rectTransform.localScale = new Vector3(1, 1, 1);
        // 设置高为此对象的高度，宽不设置，自动适配
        rectTransform.sizeDelta = new Vector2(0, ((RectTransform)this.transform).rect.height);
        // 添加水平布局组件
        menuContainer.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
        // 取消控制子对象大小
        menuContainer.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>().childControlWidth = false;
        menuContainer.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>().childControlHeight =false;
        // 添加内容大小适配组件
        menuContainer.AddComponent<UnityEngine.UI.ContentSizeFitter>().horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

        // 创建下拉菜单的容器
        GameObject dropdownContainer = new GameObject("DropdownContainer");
        dropdownContainer.transform.SetParent(this.transform);
        // 添加RectTransform并设置位置
        RectTransform dropdownRect = dropdownContainer.AddComponent<RectTransform>();
        dropdownRect.anchorMin = new Vector2(0, 1);
        dropdownRect.anchorMax = new Vector2(0, 1);
        dropdownRect.anchoredPosition = new Vector2(0, 0);
        // 设置轴心点为左上角
        dropdownRect.pivot = new Vector2(0, 1);
        // 设置下拉菜单容器的缩放
        dropdownRect.localScale = new Vector3(1, 1, 1);


        // TODO: 根据menuItems生成顶部菜单
        foreach (var menuItem in menuItems)
        {
            // 为每个菜单项创建一个按钮
            GameObject buttonObject = new GameObject(menuItem.title);
            RectTransform buttonRectTransform = buttonObject.AddComponent<RectTransform>();
            buttonObject.transform.SetParent(menuContainer.transform);
            // 设置缩放
            buttonObject.transform.localScale = new Vector3(1, 1, 1);
            // 添加内容适配组件
            buttonObject.AddComponent<UnityEngine.UI.ContentSizeFitter>().horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
            // 添加横向布局组件
            buttonObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
            // 设置填充，左右各10
            buttonObject.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>().padding = new RectOffset(10, 10, 0, 0);
            // 添加 Image 组件用于按钮背景
            var buttonImage = buttonObject.AddComponent<UnityEngine.UI.Image>();
            buttonImage.color = new Color(0, 0, 0, 1f); // 黑色
            // 添加事件系统组件
            var eventTrigger = buttonObject.AddComponent<EventTrigger>();
            // 鼠标进入
            var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryEnter.callback.AddListener((data) => OnTopMenuPointerEnter(menuItem, buttonObject));
            eventTrigger.triggers.Add(entryEnter);

            // 鼠标离开
            var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            entryExit.callback.AddListener((data) => OnTopMenuPointerExit(menuItem, buttonObject));
            eventTrigger.triggers.Add(entryExit);

            // 鼠标点击
            var entryClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            entryClick.callback.AddListener((data) => OnTopMenuPointerClick(menuItem, buttonObject));
            eventTrigger.triggers.Add(entryClick);


            // 添加新的对象用于显示TMP文本
            GameObject textObject = new GameObject("Text");
            RectTransform textRectTransform = textObject.AddComponent<RectTransform>();


            textObject.transform.SetParent(buttonObject.transform);
            var text = textObject.AddComponent<TMPro.TextMeshProUGUI>();
            text.text = menuItem.title;
            // 设置轴心点为左上角
            buttonRectTransform.pivot = new Vector2(0, 1);
            buttonRectTransform.anchoredPosition = new Vector2(0, 0);
            // 设置字体大小
            text.fontSize = 10;
            // 设置颜色
            text.color = Color.white;
            // 设置内容适配组件
            textObject.AddComponent<UnityEngine.UI.ContentSizeFitter>().horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
            // 设置高度为脚本所在对象的高度
            RectTransform parentRectTransform = this.GetComponent<RectTransform>();
            RectTransform rectTransform1 = buttonObject.GetComponent<RectTransform>();
            if (parentRectTransform != null)
            {
                rectTransform1.sizeDelta = new Vector2(rectTransform1.sizeDelta.x, parentRectTransform.rect.height);
            }
            // 设置文本对象的缩放
            textRectTransform.localScale = new Vector3(1, 1, 1);

            //rectTransform1.sizeDelta = new Vector2(rectTransform1.sizeDelta.x, this.sizeDelta.y);


            // 创建下拉菜单的容器Panel
            GameObject dropdown = new GameObject("Dropdown:"+ menuItem.title);
            dropdown.transform.SetParent(dropdownContainer.transform);
            // 添加RectTransform并设置位置
            RectTransform sunDropdownRect = dropdown.AddComponent<RectTransform>();
            sunDropdownRect.anchorMin = new Vector2(0, 1);
            sunDropdownRect.anchorMax = new Vector2(0, 1);
            // 设置轴心点为左上角
            sunDropdownRect.pivot = new Vector2(0, 1);
            // 设置缩放
            sunDropdownRect.localScale = new Vector3(1, 1, 1);
            // 在下一帧设置位置在按钮下方
            StartCoroutine(InitDropdownPosition(buttonObject, sunDropdownRect, rectTransform));
            // 设置宽度自动适配，高度自动适配
            sunDropdownRect.sizeDelta = new Vector2(0, 0);
            // 添加垂直布局组件
            dropdown.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            // 添加内容大小适配组件（宽度，高度）
            dropdown.AddComponent<UnityEngine.UI.ContentSizeFitter>().horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
            dropdown.GetComponent<UnityEngine.UI.ContentSizeFitter>().verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
            // 添加Image组件用于背景，设置为不透明黑色
            var image = dropdown.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0, 0, 0, 1); // 不透明黑色
            dropdown.SetActive(false); // 默认隐藏下拉菜单
            menuItem.TopMenuSubItemPanel = dropdown;    // 保存下拉菜单对象引用
            // 添加事件系统组件
            var dropdownEventTrigger = dropdown.AddComponent<EventTrigger>();
            // 鼠标进入
            var dropdownEntryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            dropdownEntryEnter.callback.AddListener((data) => { isPointerOverDropdown = true;   });
            dropdownEventTrigger.triggers.Add(dropdownEntryEnter);
            // 鼠标离开
            var dropdownEntryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            dropdownEntryExit.callback.AddListener((data) => { isPointerOverDropdown = false;  });
            dropdownEventTrigger.triggers.Add(dropdownEntryExit);

            // 添加子菜单的处理
            foreach (var subItem in menuItem.options)
            {
                // 创建子菜单项
                GameObject subItemObject = new GameObject(subItem.title);
                subItemObject.transform.SetParent(dropdown.transform);
                // 添加内容适配组件
                subItemObject.AddComponent<UnityEngine.UI.ContentSizeFitter>().horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
                // 添加横向布局组件
                subItemObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
                // 设置填充，左右各10
                subItemObject.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>().padding = new RectOffset(10, 10, 2, 2);
                // 设置缩放
                subItemObject.transform.localScale = new Vector3(1, 1, 1);
                // 添加 Image 组件用于背景
                var subItemImage = subItemObject.AddComponent<UnityEngine.UI.Image>();
                subItemImage.color = new Color(0, 0, 0, 1f); // 黑色
                // 添加事件系统组件
                var subEventTrigger = subItemObject.AddComponent<EventTrigger>();
                // 鼠标进入
                var subEntryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
                subEntryEnter.callback.AddListener((data) => OnButtonPointerEnter(subItem, subItemObject));
                subEventTrigger.triggers.Add(subEntryEnter);
                // 鼠标离开
                var subEntryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
                subEntryExit.callback.AddListener((data) => OnButtonPointerExit(subItem, subItemObject));
                subEventTrigger.triggers.Add(subEntryExit);
                // 鼠标点击
                var subEntryClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
                subEntryClick.callback.AddListener((data) => OnButtonPointerClick(subItem, subItemObject));
                subEventTrigger.triggers.Add(subEntryClick);

                // 添加新的对象用于显示TMP文本
                GameObject subTextObject = new GameObject("Text");
                subTextObject.transform.SetParent(subItemObject.transform);
                var subText = subTextObject.AddComponent<TMPro.TextMeshProUGUI>();
                subText.text = subItem.title;
                // 设置字体大小
                subText.fontSize = 10;
                // 设置颜色
                subText.color = Color.white;
                // 设置内容适配组件
                subTextObject.AddComponent<UnityEngine.UI.ContentSizeFitter>().horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
                // 设置缩放
                subTextObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    /// <summary>
    /// 协程用于延时创建菜单
    /// </summary>
    /// <param name="buttonObject">下拉列表的对齐目标按钮</param>
    /// <param name="dropdownRect">下拉列表的 RectTransform</param>
    /// <param name="parentRect">顶部栏对象的 RectTransform</param>
    /// <returns></returns>
    private IEnumerator InitDropdownPosition(GameObject buttonObject, RectTransform dropdownRect, RectTransform parentRect)
    {
        // 等待3帧，布局系统会在这一帧更新位置，在下一帧跳过用于防止位置闪烁，再下一帧获取位置
        yield return null;
        yield return null;
        yield return null;
        // 获取按钮的RectTransform
        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        //Debug.Log($"设置下拉菜单位置，按钮({buttonObject})位置：{buttonRect.anchoredPosition}, 顶部栏高度：{parentRect.rect.height}");
        // 设置下拉菜单位置为按钮下方
        dropdownRect.anchoredPosition = new Vector2(buttonRect.anchoredPosition.x, -parentRect.rect.height);
    }

    // 顶部菜单事件处理：鼠标进入时触发
    private void OnTopMenuPointerEnter(TopMenuItem item, GameObject button)
    {
        //Debug.Log($"鼠标进入按钮: {item.title}");
        // 显示下拉菜单等逻辑
        hoveredMenuItem = item;
        // 高亮颜色，将其改为灰色
        var image = button.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.color = new Color(0.5f, 0.5f, 0.5f, 1f); // 灰色
        }
    }
    // 顶部菜单事件处理：鼠标离开时触发
    private void OnTopMenuPointerExit(TopMenuItem item, GameObject button)
    {
        //Debug.Log($"鼠标离开按钮: {item.title}");
        // 隐藏下拉菜单等逻辑
        hoveredMenuItem = null;
        // 恢复原始颜色
        var image = button.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.color = new Color(0, 0, 0, 1f); // 黑色
        }
    }
    // 顶部菜单事件处理：鼠标点击时触发
    private void OnTopMenuPointerClick(TopMenuItem item, GameObject button)
    {
        Debug.Log($"点击按钮: {item.title}");
        // 处理点击逻辑
        // 切换下拉菜单显示状态
        if (item.TopMenuSubItemPanel != null)
        {
            bool isActive = item.TopMenuSubItemPanel.activeSelf;
            item.TopMenuSubItemPanel.SetActive(!isActive);
            // 记录当前激活的菜单项
            if (!isActive)
            {
                activeMenuItem = item;
            }
            else
            {
                activeMenuItem = null;
            }
        }
    }


    // 子菜单事件处理：鼠标进入时触发
    private void OnButtonPointerEnter(TopMenuSubItem item, GameObject button)
    {
        //Debug.Log($"鼠标进入按钮: {item.title}");
        // 显示下拉菜单等逻辑

        // 高亮颜色，将其改为灰色
        var image = button.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.color = new Color(0.5f, 0.5f, 0.5f, 1f); // 灰色
        }
    }
    // 子菜单事件处理：鼠标离开时触发
    private void OnButtonPointerExit(TopMenuSubItem item, GameObject button)
    {
        //Debug.Log($"鼠标离开按钮: {item.title}");
        // 隐藏下拉菜单等逻辑

        // 恢复原始颜色
        var image = button.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.color = new Color(0, 0, 0, 1f); // 黑色
        }
    }
    
    // 子菜单事件处理：鼠标点击时触发
    private void OnButtonPointerClick(TopMenuSubItem item, GameObject button)
    {
        //Debug.Log($"点击按钮: {item.title}");
        // 处理点击逻辑
        // 通过反射调用TopMenuF中的静态方法
        if (!string.IsNullOrEmpty(item.action))
        {
            var method = typeof(TopMenuF).GetMethod(
                item.action,
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic
            );
            if (method != null)
            {
                method.Invoke(null, null); // 静态方法，第一个参数为null
            }
            else
            {
                Debug.LogWarning($"未找到 TopMenuF 的静态方法: {item.action}");
            }
        }
    }

    void Update()
    {
        // 检测鼠标左键是否点击
        if (Input.GetMouseButtonDown(0))
        {            
            // 判断当前点击位置是否是在已打开的下拉菜单所属的 TopMenuItem 上
            if (hoveredMenuItem != null && activeMenuItem != null && hoveredMenuItem == activeMenuItem)
            {
                // 如果是则跳过这次逻辑更新
                return;
            }
            // 如果点击位置不在任何下拉菜单上，关闭所有下拉菜单
            if (!isPointerOverDropdown && activeMenuItem != null)
            {
                foreach (var menuItem in menuItems)
                {
                    if (menuItem.TopMenuSubItemPanel != null)
                    {
                        menuItem.TopMenuSubItemPanel.SetActive(false);
                    }
                }
                activeMenuItem = null;
            }
        }
    }

}
