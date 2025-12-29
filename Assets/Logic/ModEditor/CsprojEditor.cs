using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;


/// <summary>
/// 提供对 .csproj 文件的增删查改操作
/// </summary>
public class CsProjManager
{
    // 项目路径
    private readonly string _filePath;
    private readonly XDocument _doc;

    /// <summary>
    /// 构造方法，加载指定路径的 .csproj 文件
    /// </summary>
    public CsProjManager(string csprojPath)
    {
        if (!File.Exists(csprojPath))
            throw new FileNotFoundException("指定的 .csproj 文件不存在。", csprojPath);

        _filePath = csprojPath;
        _doc = XDocument.Load(_filePath);
    }

    /// <summary>
    /// 保存修改后的文件
    /// </summary>
    public void Save()
    {
        _doc.Save(_filePath);
    }

    #region 查询操作
    /// <summary>
    /// 获取所有 （Compile/编译） 项
    /// </summary>
    public IEnumerable<string> GetCompileItems()
    {
        return _doc.Root?
                    .Elements("ItemGroup")
                    .Elements("Compile")
                    .Select(e => e.Attribute("Include")?.Value)
                    .Where(v => !string.IsNullOrEmpty(v)) ?? Enumerable.Empty<string>();
    }

    /// <summary>
    /// 获取所有 （EmbeddedResource/内嵌资源） 项
    /// </summary>
    public IEnumerable<string> GetEmbeddedResources()
    {
        return _doc.Root?
                   .Elements("ItemGroup")
                   .Elements("EmbeddedResource")
                   .Select(e => e.Attribute("Include")?.Value)
                   .Where(v => !string.IsNullOrEmpty(v)) ?? Enumerable.Empty<string>();
    }

    /// <summary>
    /// 获取所有 （Package包） 项
    /// </summary>
    public IEnumerable<(string Include, string Version)> GetPackageReferences()
    {
        return _doc.Root?
                    .Elements("ItemGroup")
                    .Elements("PackageReference")
                    .Select(e => (
                        Include: e.Attribute("Include")?.Value,
                        Version: e.Attribute("Version")?.Value
                    ))
                    .Where(x => !string.IsNullOrEmpty(x.Include)) ?? Enumerable.Empty<(string, string)>();
    }
    #endregion

    #region 新增操作
    /// <summary>
    /// 添加一个 （Compile/编译） 项
    /// <para>提示：.cs会被默认包含其中，无需手动管理</para>
    /// </summary>
    public void AddCompileItem(string includePath)
    {
        var itemGroup = _doc.Root?
                            .Elements("ItemGroup")
                            .FirstOrDefault() ?? new XElement("ItemGroup");

        if (itemGroup.Parent == null)
            _doc.Root?.Add(itemGroup);

        itemGroup.Add(new XElement("Compile", new XAttribute("Include", includePath)));
    }

    /// <summary>
    /// 添加一个（EmbeddedResource/内嵌资源）项
    /// </summary>
    public void AddEmbeddedResource(string includePath)
    {
        var itemGroup = _doc.Root?
                            .Elements("ItemGroup")
                            .FirstOrDefault() ?? new XElement("ItemGroup");

        if (itemGroup.Parent == null)
            _doc.Root?.Add(itemGroup);

        itemGroup.Add(new XElement("EmbeddedResource", new XAttribute("Include", includePath)));
    }



    /// <summary>
    /// 添加一个（Package包）项
    /// </summary>
    public void AddPackageReference(string include, string version)
    {
        var itemGroup = _doc.Root?
                            .Elements("ItemGroup")
                            .FirstOrDefault() ?? new XElement("ItemGroup");

        if (itemGroup.Parent == null)
            _doc.Root?.Add(itemGroup);

        itemGroup.Add(new XElement("PackageReference",
            new XAttribute("Include", include),
            new XAttribute("Version", version)));
    }
    #endregion

    #region 删除操作
    /// <summary>
    /// 删除指定路径的（Compile/编译）项
    /// </summary>
    public bool RemoveCompileItem(string includePath)
    {
        var elements = _doc.Root?
                            .Elements("ItemGroup")
                            .Elements("Compile")
                            .Where(e => e.Attribute("Include")?.Value == includePath)
                            .ToList();

        if (!elements.Any()) return false;

        foreach (var elem in elements)
            elem.Remove();

        return true;
    }

    /// <summary>
    /// 删除指定路径的（EmbeddedResource/内嵌资源）项
    /// </summary>
    public bool RemoveEmbeddedResource(string includePath)
    {
        var elements = _doc.Root?
                           .Elements("ItemGroup")
                           .Elements("EmbeddedResource")
                           .Where(e => e.Attribute("Include")?.Value == includePath)
                           .ToList();

        if (!elements.Any()) return false;

        foreach (var elem in elements)
            elem.Remove();

        return true;
    }

    /// <summary>
    /// 删除指定Package包名的 PackageReference 项
    /// </summary>
    public bool RemovePackageReference(string include)
    {
        var elements = _doc.Root?
                            .Elements("ItemGroup")
                            .Elements("PackageReference")
                            .Where(e => e.Attribute("Include")?.Value == include)
                            .ToList();

        if (!elements.Any()) return false;

        foreach (var elem in elements)
            elem.Remove();

        return true;
    }
    #endregion

    #region 修改操作
    /// <summary>
    /// 修改指定 Package包 的版本号
    /// </summary>
    public bool UpdatePackageReferenceVersion(string include, string newVersion)
    {
        var element = _doc.Root?
                            .Elements("ItemGroup")
                            .Elements("PackageReference")
                            .FirstOrDefault(e => e.Attribute("Include")?.Value == include);

        if (element == null) return false;

        element.SetAttributeValue("Version", newVersion);
        return true;
    }

    /// <summary>
    /// 修改指定 内嵌资源 的路径
    /// </summary>
    /// <param name="oldIncludePath">原路径</param>
    /// <param name="newIncludePath">新路径</param>
    /// <returns>是否找到并修改成功</returns>
    public bool UpdateEmbeddedResourcePath(string oldIncludePath, string newIncludePath)
    {
        var element = _doc.Root?
                          .Elements("ItemGroup")
                          .Elements("EmbeddedResource")
                          .FirstOrDefault(e => e.Attribute("Include")?.Value == oldIncludePath);

        if (element == null) return false;

        element.SetAttributeValue("Include", newIncludePath);
        return true;
    }
    #endregion
}
