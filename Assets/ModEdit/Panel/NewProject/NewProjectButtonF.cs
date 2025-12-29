using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NewProjectButtonF : MonoBehaviour
{
    // 输入字段组件（TMP）：项目名称
    public TMPro.TMP_InputField projectNameInput;
    // 下拉菜单组件（TMP）
    public TMPro.TMP_Dropdown projectTypeDropdown;
    // 输入字段组件（TMP）：项目地址
    public TMPro.TMP_InputField projectPathInput;
    // Text组件（TMP）：项目地址提示
    public TMPro.TMP_Text projectPathHintText;

    // 第一步的面板对象
    public GameObject stepOnePanel;
    // 第二步的面板对象
    public GameObject stepTwoPanel;
    // 点击下一步的事件
    public void OnNextButtonClick()
    {
        // 处理下一步按钮点击事件
        // 检查输入字段是否为空
        string projectName = projectNameInput.text;
        if (string.IsNullOrEmpty(projectName))
        {
            Debug.LogWarning("项目名称不能为空！");
            return;
        }
        // 获取选择的项目类型
        int projectTypeIndex = projectTypeDropdown.value;
        string projectType = projectTypeDropdown.options[projectTypeIndex].text;
        Debug.Log($"创建新项目：名称={projectName}, 类型={projectType}");
        // 打开stepTwoPanel
        stepOnePanel.SetActive(false);
        stepTwoPanel.SetActive(true);
    }

    // 点击取消的事件
    public void OnCancelButtonClick()
    {
        // 处理取消按钮点击事件
        Debug.Log("取消新建项目");
        // 删除新建项目面板
        Destroy(gameObject);
    }

    // 打开文件夹选择对话框的事件
    public void OnBrowseButtonClick()
    {
        Debug.Log("打开文件夹选择对话框");
        var path = SFB.StandaloneFileBrowser.OpenFolderPanel("选择项目文件夹", "", false);

        Debug.Log("选择的文件路径: " + path[0]);
        projectPathInput.text = path[0].ToString();

        // 使用 Path.Combine 构建路径
        string projectRoot = path[0].ToString();
        string projectName = projectNameInput.text;
        string fullPath = System.IO.Path.Combine(projectRoot, projectName);
        projectPathHintText.text = "项目将在: " + fullPath + "  下创建";
    }

    // 点击创建项目的事件
    public void OnCreateButtonClick()
    {
        // 处理创建项目按钮点击事件
        string projectName = projectNameInput.text;
        string projectPath = projectPathInput.text;

        if (string.IsNullOrEmpty(projectPath))
        {
            Debug.LogWarning("项目路径不能为空！");
            return;
        }

        string fullPath = System.IO.Path.Combine(projectPath, projectName);
        Debug.Log($"创建项目：名称={projectName}, 路径={fullPath}");

        // 在这里添加实际的项目创建逻辑
        // 使用多线程创建
        Task.Run(() => CreateProject(projectName, fullPath));
        // 删除新建项目面板
        Destroy(gameObject);
    }





    // 创建项目的方法
    private async void CreateProject(string projectName, string projectPath)
    {
        // 实现项目创建的逻辑
        Debug.Log($"创建项目：名称={projectName}, 路径={projectPath}");
        // 创建项目文件夹
        System.IO.Directory.CreateDirectory(projectPath);
        // 创建csproj文件，目标框架是netstandard2.1
        string csprojContent = $@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
  </PropertyGroup>
</Project>";
        string csprojPath = System.IO.Path.Combine(projectPath, projectName + ".csproj");
        System.IO.File.WriteAllText(csprojPath, csprojContent);
        Debug.Log($"项目文件创建完成：{csprojPath}");
        
        await Task.CompletedTask;
    }
}
