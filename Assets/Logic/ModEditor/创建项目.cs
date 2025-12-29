using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.UI;
using System.IO;
using System;
using System.Xml.Linq;
using System.Linq;

public class 创建项目 : MonoBehaviour
{
    public InputField 项目名称;
    public InputField 项目地址;

    public GameObject GameObject;

    public void 新建项目确认按钮()
    {
        Debug.Log("项目名称为：" + 项目名称.text);
        Debug.Log("项目地址为：" + 项目地址.text);

        创建源文件(项目地址.text, 项目名称.text);
        项目名称.text = "";
        项目地址.text = "";


    }

    public void 新建项目取消按钮()
    {
        // 关闭这个板
        GameObject.SetActive(false);
        // 清空已输入的内容
        项目名称.text = "";
        项目地址.text = "";
    }

    public void 打开新建项目面板()
    {
        // 打开这个板
        GameObject.SetActive(true);
        // 清空已输入的内容
        项目名称.text = "";
        项目地址.text = "";
    }

    public void 选择项目路径()
    {
        //var extensions = new[] {
        //new ExtensionFilter("原版素材文件", "vxl", "shp"),
        //new ExtensionFilter("支持的自定义文件", "fbx", "obj","png","jpg"),
        //};

        //var paths = StandaloneFileBrowser.OpenFilePanel("O选择项目根目录", "", extensions, false);
        var path = StandaloneFileBrowser.OpenFolderPanel("选择项目文件夹", "", false);
        // 调用 OpenFilePanel 方法

        Debug.Log("选择的文件路径: " + path[0]);
        项目地址.text = path[0].ToString();
        //string selectedPath = OpenFolderPanel("选择项目根目录", "");
        //if (!string.IsNullOrEmpty(selectedPath))
        //{
        //    项目地址.text = selectedPath;
        //}
    }

    public void 检查是否可以创建()
    {

    }



    // 以下是项目创建部分

    /// <summary>
    /// 
    /// </summary>
    /// <param name="projectPath">项目路径 项目根目录</param>
    /// <param name="projectName">项目名称</param>
    public void 创建源文件(string projectPath,string projectName)
    {
        // 创建项目 .csproj 文件
        string projectFile = Path.Combine(projectPath, $"{projectName}.csproj");

        // 创建项目目录
        Directory.CreateDirectory(projectPath);

        // 创建项目文件内容
        string projectContent = CreateProjectFileContent(projectName);

        // 保存项目文件
        File.WriteAllText(projectFile, projectContent);

        Debug.Log($"项目文件已创建：{projectFile}");

        // 添加代码文件
        string codeFilePath = Path.Combine(projectPath, "ModMain.cs");
        string codeContent = $@"
using FDATE;

namespace {projectName}
{{
    public class Mod
    {{
        // Mod入口，在开始时会调用一次该方法
        public void ModMain()
        {{
            ModToolkit.TestDebug(""Hello F&D Adjudgment to Empire!"");
        }}

        // 游戏逻辑更新时会调用此方法，调用频率是20TPS
        public void ModFixedUpdate()
        {{
            
        }}

        // 游戏画面每更新一帧时调用一次
        public void ModUpdate()
        {{
            
        }}
    }}
}}
";
        File.WriteAllText(codeFilePath, codeContent);

        Debug.Log($"代码文件已添加：{codeFilePath}");

        // 添加引用的DLL
        string dllPath = GetStreamingAssetsPath("netstandard2.1\\FDATE.dll");
        AddReferenceToProject(projectFile, dllPath);

        Debug.Log($"DLL引用已添加：{dllPath}");



    }

    static string CreateProjectFileContent(string projectName)
    {
        // 创建项目文件内容
        XDocument projectDoc = new XDocument(
            new XElement("Project",
                new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                new XElement("PropertyGroup",
                    new XElement("TargetFramework", "netstandard2.1"),
                    new XElement("AssemblyName", projectName)
                ),
                new XElement("ItemGroup",
                    new XElement("Compile",
                        new XAttribute("Include", "ModMain.cs")
                    )
                )
            )
        );

        return projectDoc.ToString();
    }


    static void AddReferenceToProject(string projectFile, string dllPath)
    {
        // 加载项目文件
        XDocument projectDoc = XDocument.Load(projectFile);

        // 添加引用
        XElement itemGroup = projectDoc.Descendants("ItemGroup").FirstOrDefault();
        if (itemGroup == null)
        {
            itemGroup = new XElement("ItemGroup");
            projectDoc.Root.Add(itemGroup);
        }

        XElement reference = new XElement("Reference",
            new XAttribute("Include", Path.GetFileNameWithoutExtension(dllPath)),
            new XElement("HintPath", dllPath)
        );

        itemGroup.Add(reference);

        // 保存项目文件
        projectDoc.Save(projectFile);
    }



    // 获取StreamingAssets路径
    static string GetStreamingAssetsPath(string fileName)
    {
        string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, fileName);
        return streamingAssetsPath;
    }
}



