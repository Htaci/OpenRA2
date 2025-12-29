using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.IO;
using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

/// <summary>
/// 负责管理mod的类
/// </summary>
public static class ModAssembly
{
    /// <summary>
    /// Mod 程序集依赖项，在游戏App启动时就会加载并进行初始化。
    /// <para>Mod程序集依赖项 : Mod实现调用游戏内的方法的原理是基于反射的。</para>
    /// <para>而此工具库代替了社区开发者们实现了反射调用的过程，并进行了优化，</para>
    /// <para>因此此库必须在任何Mod加载前加载，才可以保证Mod正常运行</para>
    /// </summary>
    public static Assembly dependenciesAssembly;

    /// <summary>
    /// Mod 列表，负责管理所有的 Mod 类库，可以动态加载和卸载。
    /// <para>已经存档的行动记录重新进入时会强制启动该记录在开始游玩时使用的Mod，</para>
    /// <para>以确保存档不会因为缺少Mod坏档。</para>
    /// <para>#如果没有下载该Mod或Mod下架，则会弹出警告，确认是否在缺少Mod的情况下打开此行动存档</para>
    /// </summary>
    public static List<ModClass> modAssembly = new List<ModClass>();

    // 构造函数
    static ModAssembly()
    {
        Debug.Log("ModAssembly构造函数 : 加载 FDATE.dll 类库");
        string dllPath = Toolkit.GetAssetPath("netstandard2.1", "FDATE.dll");
        //dependenciesAssembly = Assembly.LoadFrom(dllPath);
    }
    /// <summary>
    /// 加载 Mod 到列表中
    /// </summary>
    /// <param name="path"></param>
    public static void LoadModCoroutine(string path)
    {
        Debug.Log($"ModAssembly/Mod加载函数 : 加载Mod类库（{path}）");
        ModClass mod = new ModClass
        {
            assembly = Assembly.LoadFrom(path)
        };
        // 获取 Mod 类型
        Type modType = mod.assembly.GetType($"FD_AdjudgmenttoEmpire_Mod.Mod");

        if (modType != null)
        {
            // 初始化 Mod 实例
            mod.ModInstance = Activator.CreateInstance(modType);
            mod.modType = modType;
        }
        else
        {
            Debug.Log($"ModAssembly/Mod加载函数 : Mod 类型为空，取消加载");
            return;
        }
        
        // 将Mod类库加载到ModAssembly集中管理
        modAssembly.Add(mod);
        Debug.Log($"ModAssembly/Mod加载函数 : 已加载到列表中");
    }

    /// <summary>
    /// 调用所有 Mod 的 Main 方法
    /// </summary>
    public static void StartCalling()
    {
        foreach (ModClass mod in modAssembly)
        {
            // 获取 ModMain 方法
            MethodInfo modMainMethod = mod.modType.GetMethod("ModMain");
            try
            {
                // 调用 Mod 的代码
                modMainMethod.Invoke(mod.ModInstance, null);
            }
            catch (Exception ex)
            {
                //Debug.LogError($"Mod执行错误: {ex.Message}");
                // 错误日志
                Debug.LogError($"Mod加载失败: {ex.GetType().Name}");
                Debug.LogError($"错误信息: {ex.Message}");

                // 打印内部异常详情
                if (ex.InnerException != null)
                {
                    Debug.LogError($"内部异常: {ex.InnerException.GetType().Name}");
                    Debug.LogError($"内部错误信息: {ex.InnerException.Message}");
                    Debug.LogError($"堆栈跟踪: {ex.InnerException.StackTrace}");
                }
            }
        }
    }

    /// <summary>
    /// 加载程序集内的资源
    /// </summary>
    /// <param name="assembly">目标程序集</param>
    /// <param name="resourceName">目标文件名</param>
    public static void LoadResource(Assembly assembly)
    {
        // 加载 Mod 配置文件
        string modConfigInfo = Toolkit.ConvertStreamToString(LoadFilel(assembly,"ModConfigInfo.json"));
        // 反序列化
        ModConfig modConfig = JsonUtility.FromJson<ModConfig>(modConfigInfo);
    }

    /// <summary>
    /// 查找程序集内的文件
    /// </summary>
    /// <param name="assembly">目标程序集</param>
    /// <param name="resourceName">目标文件名</param>
    /// <returns>返回文件的内存流</returns>
    public static Stream LoadFilel(Assembly assembly, string resourceName)
    {
        string resourcePath = assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith(resourceName));

        if (resourcePath == null)
        {
            Debug.LogError($"资源 '{resourceName}' 未找到。");
            return null;
        }
        else
        {
            try
            {
                // 打开嵌入资源流
                using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
                {
                    if (stream == null)
                    {
                        Debug.LogError($"无法打开资源流 '{resourceName}'。");
                        return null;
                    }
                    return stream;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"读取资源 '{resourceName}' 时发生错误: {ex.Message}");
                return null;
            }
        }

    }

}






