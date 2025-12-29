using Newtonsoft.Json;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class 编辑器按钮类 : MonoBehaviour
{
    // 
    public S_ModPack s_ModPack;

    // 
    public S_UnitUID s_UnitUID;

    public Image image;//用来显示图片

    public string selectedFilePath;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void 导入新资产()
    {

    }

    public void 打开列表(GameObject a)
    {
        if (a.activeSelf == false)
        {
            a.SetActive(true);
        }
        else
        {
            a.SetActive(false);
        }
    }

    public void OpenFile()
    {
        // 调用 OpenFilePanel 方法
        var selectedFilePath = StandaloneFileBrowser.OpenFilePanel("选择文件", "", "",false);
        Debug.Log("选择的文件路径: " + selectedFilePath[0]);
    }

    // 把模型加载到场景里
    public void LoadFile()
    {
        ResourcesCacheManager.Instance.LoadModelFromFile(selectedFilePath, "a.fbx");// "D:\Unity\FD-ATE\Assets\StreamingAssets\战斗碉堡.fbx"Assets/StreamingAssets/战斗碉堡.fbx
    }

    // 输出所有已经缓存的模型
    public void LogModelDictionary()
    {
        foreach (var a in ResourcesCacheManager.Instance.modelCache)
        {
            Debug.Log("缓存模型名：" + a.Key);
        }
    }

    // 测试方法：按钮触发方法，运行时加载外部图片
    public void LoadImageTest()
    {
        Sprite sprite = TextureToSprite (LoadTextureByIO(selectedFilePath));
        image.sprite = sprite;
    }

    // 按钮触发，加载项目
    public void LoadProject(string projectFilePath)
    {
        Debug.Log("模组编辑器:正在加载项目...");

        LoadProjectAsync(projectFilePath);
    }
    // 异步加载
    public async Task LoadProjectAsync(string projectFilePath)
    {
        // 检查路径是否存在
        if (!Directory.Exists(projectFilePath))
        {
            Debug.Log("指定的路径不存在: " + projectFilePath);
            return;
        }

        // 获取指定路径下的所有文件，包括子文件夹中的文件
        string[] allFiles = Directory.GetFiles(projectFilePath, "*.*", SearchOption.AllDirectories);

        bool isFilemain = false;

        // 遍历所有文件
        foreach (string file in allFiles)
        {
            // 获取文件名
            string fileName = Path.GetFileName(file);

            // 判断文件名是否为 "main.json"
            if (fileName == "main.json")
            {
                isFilemain = true;
                Debug.Log("找到文件: " + file);
                // 在这里可以添加对 "main.json" 文件的处理逻辑
                // 读取文件内容
                string content = File.ReadAllText(file);

                // 使用 JsonReader 读取内容
                using (JsonReader reader = new JsonTextReader(new StringReader(content)))
                {
                    // 反序列化 JSON 数据
                    s_ModPack = JsonSerializer.Create().Deserialize<S_ModPack>(reader);
                }
            }
        }


        // 判断是否找到项目配置
        if (!isFilemain)
        {
            Debug.Log("模组编辑器:没有找到项目配置");
        }
        else
        {
            // 找到了项目配置，开始初始化
            
        }

        

    }

    // 加载单位属性（类原版中定义单位属性的ini）
    public void LoadUnitdate(string content)
    {
        using (JsonReader reader = new JsonTextReader(new StringReader(content)))
        {
            // 反序列化 JSON 数据
            //s_ModPack = JsonSerializer.Create().Deserialize<S_UnitUID>(reader);
        }
    }



    /// <summary>
    /// 从外部指定文件中加载图片
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <returns></returns>
    private Texture2D LoadTextureByIO(string path)
    {
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        fs.Seek(0, SeekOrigin.Begin);//游标的操作，可有可无
        byte[] bytes = new byte[fs.Length];//生命字节，用来存储读取到的图片字节
        try
        {
            fs.Read(bytes, 0, bytes.Length);//开始读取，这里最好用trycatch语句，防止读取失败报错

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        fs.Close();//切记关闭

        int width = 2048;//图片的宽（这里两个参数可以提到方法参数中）
        int height = 2048;//图片的高（这里说个题外话，pico相关的开发，这里不能大于4k×4k不然会显示异常，当时开发pico的时候应为这个问题找了大半天原因，因为美术给的图是6000*3600，导致出现切几张图后就黑屏了。。。
        Texture2D texture = new Texture2D(width, height);
        if (texture.LoadImage(bytes))
        {
            print("图片加载完毕 ");
            return texture;//将生成的texture2d返回，到这里就得到了外部的图片，可以使用了

        }
        else
        {
            print("图片尚未加载");
            return null;
        }
    }

    /// <summary>
    /// 将Texture2d转换为Sprite
    /// </summary>
    /// <param name="tex">参数是texture2d纹理</param>
    /// <returns></returns>
    private Sprite TextureToSprite(Texture2D tex)
    {
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }

}
