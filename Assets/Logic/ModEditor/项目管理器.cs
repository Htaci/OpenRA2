using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class 项目管理器
{
    // 已打开的项目名称
    public string openProjectName;

    // 已打开项目的路径
    public string openProjectPath;

    CsProjManager csproj;

    // Start is called before the first frame update
    //void Start()
    //{
    //    csproj = new CsProjManager("D:/Unity/FD-ATE/TestMod/testmod.csproj");

    //    // 查询
    //    Debug.Log("内嵌资源有:");
    //    foreach (var item in csproj.GetEmbeddedResources())
    //        Debug.Log($"内嵌资源- {item}");

    //    Debug.Log("编译文件有:");
    //    foreach (var item in csproj.GetCompileItems())
    //        Debug.Log($"编译文件- {item}");

    //    //Debug.Log("\nPackage references:");
    //    //foreach (var (pkg, ver) in csproj.GetPackageReferences())
    //    //    Debug.Log($"- {pkg} v{ver}");

    //    // 新增
    //    //csproj.AddCompileItem("NewFile.cs");
    //    //csproj.AddPackageReference("Newtonsoft.Json", "13.0.3");

    //    // 删除
    //    //csproj.RemoveCompileItem("OldFile.cs");
    //    //csproj.RemovePackageReference("Obsolete.Package");

    //    // 修改
    //    //csproj.UpdatePackageReferenceVersion("Newtonsoft.Json", "14.0.0");

    //    // 保存
    //    //csproj.Save();
    //}
}
