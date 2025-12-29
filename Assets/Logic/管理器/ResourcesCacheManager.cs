using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TriLibCore;
using UnityEngine;

// 资源缓存管理器
public class ResourcesCacheManager : MonoBehaviour
{
    // 单例模式
    public static ResourcesCacheManager Instance { get; private set; }

    // 缓存字典，存储模型名 - > 对应的内存流
    public Dictionary<string, MemoryStream> modelCache = new Dictionary<string, MemoryStream>();

    // 信息字典，记录了所有单位的信息
    public Dictionary<string, S_UnitUID> unitUID = new Dictionary<string, S_UnitUID>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保在场景切换时不被销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 加载整合模组包
    public void LoadModPack(string tarFilePath)
    {
        // 检查文件是否存在
        if (!File.Exists(tarFilePath))
        {
            Debug.LogError($"Tar 文件不在路径 {tarFilePath} 下。");
            return;
        }

        // 打开 tar 文件
        using (FileStream fs = File.OpenRead(tarFilePath))
        using (TarInputStream tarInput = new TarInputStream(fs, System.Text.Encoding.UTF8))
        {
            TarEntry entry;
            while ((entry = tarInput.GetNextEntry()) != null)
            {
                // 检查是否是 main.json 文件
                if (entry.Name.EndsWith("main.json", StringComparison.OrdinalIgnoreCase))
                {
                    // 读取文件内容
                    byte[] buffer = new byte[entry.Size];
                    tarInput.Read(buffer, 0, buffer.Length);

                    // 将字节数组转换为字符串
                    string iniContent = System.Text.Encoding.UTF8.GetString(buffer);

                    // 开始初始化



                    // 输出内容到控制台
                    //Debug.Log("mian.json 的内容是:");
                    //Debug.Log(iniContent);

                    // 保存到文件（可选）
                    // SaveIniToFile(buffer, "main.INI");

                    return; // 找到文件后退出循环
                }
            }
        }

        Debug.LogError("main.json not found in the tar file.");
    }

    // 从tar加载模型（检查缓存，如果没有则从tar文件加载并缓存）
    public void LoadModel(string tarFilePath, string modelName)
    {
        MemoryStream modelStream;

        // 检查是否已经缓存
        if (!modelCache.TryGetValue(modelName, out modelStream))
        {
            // 模型未缓存，从tar文件加载
            using (FileStream fs = File.OpenRead(tarFilePath))
            using (TarInputStream tarInput = new TarInputStream(fs, System.Text.Encoding.UTF8))
            {
                TarEntry entry;
                while ((entry = tarInput.GetNextEntry()) != null)
                {
                    if (entry.Name.EndsWith(modelName, StringComparison.OrdinalIgnoreCase))
                    {
                        byte[] modelData = new byte[entry.Size];
                        tarInput.Read(modelData, 0, modelData.Length);

                        // 使用自定义的非关闭内存流
                        modelStream = new NonClosingMemoryStream(modelData);
                        modelCache[modelName] = modelStream; // 缓存模型

                        // 加载模型
                        LoadModelFromStream(modelStream, modelName);
                        return;
                    }
                }
            }

            Debug.LogError($"Model {modelName} not found in tar file.");
            return;
        }

        // 模型已缓存，直接加载
        LoadModelFromStream(modelStream, modelName);
    }

    // 直接加载模型（检查缓存，如果没有则从文件路径加载并缓存）
    public void LoadModelFromFile(string filePath, string modelName)
    {
        MemoryStream modelStream;

        // 检查是否已经缓存
        if (!modelCache.TryGetValue(modelName, out modelStream))
        {
            // 模型未缓存，从文件路径加载
            if (File.Exists(filePath))
            {
                byte[] modelData = File.ReadAllBytes(filePath);

                // 使用自定义的非关闭内存流
                modelStream = new NonClosingMemoryStream(modelData);
                //modelCache[modelName] = modelStream; // 缓存模型

                // 加载模型
                LoadModelFromStream(modelStream, modelName);
            }
            else
            {
                Debug.LogError($"Model file {filePath} does not exist.");
            }
        }
        else
        {
            // 模型已缓存，直接加载
            LoadModelFromStream(modelStream, modelName);
        }
    }

    // 使用TriLib加载模型
    private void LoadModelFromStream(MemoryStream modelStream, string modelName)
    {
        AssetLoader.LoadModelFromStream(
            stream: modelStream,
            filename: modelName,
            onLoad: OnModelLoad,
            onMaterialsLoad: null,
            onProgress: null,
            onError: OnError,
            wrapperGameObject: null,
            assetLoaderOptions: null,
            customContextData: null,
            haltTask: false,
            onPreLoad: null,
            isZipFile: false
        );
    }

    private void OnModelLoad(AssetLoaderContext context)
    {
        if (context.RootGameObject != null)
        {
            context.RootGameObject.transform.position = Vector3.zero;
            context.RootGameObject.transform.rotation = Quaternion.identity;
            Debug.Log("Model loaded successfully.");
        }
    }

    private void OnError(IContextualizedError error)
    {
        Debug.LogError($"加载模型时出错: {error.GetInnerException().Message}");
    }

    // 自定义非关闭内存流
    private class NonClosingMemoryStream : MemoryStream
    {
        public NonClosingMemoryStream(byte[] buffer) : base(buffer)
        {
        }

        public override void Close()
        {
            // 防止流被关闭
        }
    }


}


