using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.IO;
using UnityEngine.PlayerLoop;
using System.Linq;

public class ModLoad : MonoBehaviour
{
    /// <summary>
    /// 加载 mod 的方法，传入mod的tar包路径
    /// </summary>
    /// <param name="modPath"></param>
    public void LoadMod(string modPath)
    {
        if (!File.Exists(modPath))
        {
            Debug.LogError($"Mod file not found: {modPath}");
            return;
        }

        try
        {
            // 读取 mod 的 tar 包
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load mod: {ex.Message}");
        }
    }
}

public class ModInfo
{
    public string ModName;
    public string Version;
    public string Author;
    public string Description;

    public ModInfo(string modName, string version, string author, string description)
    {
        ModName = modName;
        Version = version;
        Author = author;
        Description = description;
    }
}
