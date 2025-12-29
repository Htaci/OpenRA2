using UnityEngine;
using SFB;
public class OpenFileDialogExample : MonoBehaviour
{
    public string selectedFilePath;

    void Start()
    {

    }

    public void OpenFile()
    {
        // 调用 OpenFilePanel 方法
        var selectedFilePath = StandaloneFileBrowser.OpenFilePanel("选择文件", "", "fbx",true);
        Debug.Log("选择的文件路径: " + selectedFilePath);
    }

    // 把模型加载到场景里
    public void LoadFile()
    {
        ResourcesCacheManager.Instance.LoadModelFromFile(selectedFilePath, "a.fbx");// "D:\Unity\FD-ATE\Assets\StreamingAssets\战斗碉堡.fbx"Assets/StreamingAssets/战斗碉堡.fbx
    }

    // 输出所有已经缓存的模型
    public void LogModelDictionary()
    {
        foreach( var a  in ResourcesCacheManager.Instance.modelCache)
        {
            Debug.Log("缓存模型名：" + a.Key);
        }
    }


}