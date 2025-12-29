using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 标题栏按钮管理器 : MonoBehaviour
{
    public List<标题栏下拉按钮> 按钮列表 = new List<标题栏下拉按钮>();

    public static 标题栏按钮管理器 Instance;
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        // 检测鼠标左键是否被点击
        if (Input.GetMouseButtonDown(0))
        {
            // 遍历按钮列表
            foreach (标题栏下拉按钮 按钮 in 按钮列表)
            {
                // 如果鼠标点击位置在按钮范围内
                if (按钮.isShow)
                {
                    按钮.触发任务();
                }
                else
                {
                    if (按钮.GameObject != null)
                    {
                        按钮.GameObject.SetActive(false);
                    }
                }
            }
        }
        
    }
}
