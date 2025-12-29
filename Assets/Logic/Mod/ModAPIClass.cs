using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ModAPIClass : MonoBehaviour
{
    // 单例模式
    public static ModAPIClass Instance { get; set; }

    //public Canvas canvas;

    public ModLoad modLoad;

    public GameObject GameObject;

    public GameObject pGameObject;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保在场景切换时不被销毁
            Debug.Log("ModAPIClass 初始化完成");
        }
        else
        {
            Destroy(gameObject);
        }

        // 加载测试MOD
        ModAssembly.LoadModCoroutine(Toolkit.GetAssetPath( "netstandard2.1", "testmod.dll"));

        ModAssembly.StartCalling();

        
    }

    /// <summary>
    /// 用于调试的输出，在游戏模式不会显示，在编辑模式下会显示在左下角(在游戏窗口内而非控制台)
    /// </summary>
    /// <param name="str">需要输出的内容</param>
    public static void TestDebug(string str)
    {
        Debug.Log("ModDebug:"+str);
        // 检查引用
        if (Instance.pGameObject == null)
        {
            Debug.Log("ModAPIClass:Instance.pGameObject为空，取消执行");
            return;
        }
        GameObject newObject = Instantiate(Instance.pGameObject);
        Text textComponent = newObject.GetComponentInChildren<Text>();
        if (textComponent != null)
        {
            textComponent.text = str;
            // 设置父对象
            if (Instance.GameObject == null)
            {
                Debug.Log("ModAPIClass:Instance.GameObject为空，取消执行");
                Destroy(newObject);
            }
            newObject.transform.SetParent(Instance.GameObject.transform, false);
        }
        else
        {
            Destroy(newObject);
        }
    }


    // 获取StreamingAssets路径
    string GetStreamingAssetsPath(string fileName)
    {
        string streamingAssetsPath = Path.Combine(Application.streamingAssetsPath, fileName);
        return streamingAssetsPath;
    }
}
