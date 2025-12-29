using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 自定义工具类
/// </summary>
public static class Toolkit
{
    /// <summary>
    /// 一个返回 StreamingAssets 文件夹下指定路径的方法，无需考虑程序位置，只需确保文件在 StreamingAssets 内（包括子文件夹）
    /// </summary>
    /// <returns>返回文件的路径，跨平台</returns>
    public static string GetAssetPath(params string[] numbers)
    {
        string path = GetStreamingAssetsPath(Path.Combine(numbers));

        return path;
    }

    /// <summary>
    /// 获取StreamingAssets路径
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetStreamingAssetsPath(string fileName)
    {
        string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, fileName);
        return streamingAssetsPath;
    }

    /// <summary>
    /// 数据转换 Stream ---> String
    /// </summary>
    /// <param name="stream"></param>
    /// <returns>返回转化的字符串</returns>
    public static string ConvertStreamToString(Stream stream)
    {
        // 将流转换为字符串
        using (StreamReader reader = new StreamReader(stream))
        {
            string resourceContent = reader.ReadToEnd();
            Debug.Log($"读取到的资源内容: {resourceContent}");
            return resourceContent;
        }
    }

    /// <summary>
    /// 向Mod项目的csproj中添加中添加文件引用并设置为嵌入资源
    /// </summary>
    /// <param name="path">项目目录</param>
    public static void AddResource(string path, string fileName)
    {
        // 查找csproj文件路径
        string csprojPath = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories)[0];

        // 在csprog中追加fileName文件的引用并设置为嵌入资源
        File.AppendAllText(csprojPath, $"<EmbeddedResource Include=\"{fileName}\" />");
    }

    // 向Mod项目中的csproj中移除指定嵌入文件
    public static void RemoveResource(string path, string fileName)
    {
        // 查找csproj文件路径
        string csprojPath = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories)[0];

        // 读取csproj文件内容
        string csprojContent = File.ReadAllText(csprojPath);
        // 移除指定嵌入文件
        csprojContent = csprojContent.Replace($"<EmbeddedResource Include=\"{fileName}\" />", "");
    }
}
