using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB; // Simple File Browser 插件的命名空间

// 此类实现了顶部菜单栏的子级按钮的触发执行方法
public class TopMenuF : MonoBehaviour
{
    // 打开新建项目的方法
    public static void OpenNewProjectPanel()
    {
        // 加载预制件放到场景中
        GameObject NewProjectPanelPrefab = Resources.Load<GameObject>("UI/EditNewProjectPanel/Panel");
        // 放到Canvas下
        GameObject canvas = GameObject.Find("Canvas");
        GameObject.Instantiate(NewProjectPanelPrefab, canvas.transform);
    }

    // 打开项目的方法（打开资源管理器并选择文件夹使用SFB插件）
    public static void OpenProject()
    {
        // 使用Simple File Browser插件打开文件夹选择对话框
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("选择项目文件夹", "", false);
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            string selectedPath = paths[0];

        }

    }
    
}
