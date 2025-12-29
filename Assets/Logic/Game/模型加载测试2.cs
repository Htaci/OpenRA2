using System.IO;
using UnityEngine;
using ICSharpCode.SharpZipLib.Tar;
using TriLibCore;
using System;

public class LoadTarModel : MonoBehaviour
{
    void Start()
    {
        ResourcesCacheManager.Instance.LoadModel("Assets/StreamingAssets/test.tar", "战斗碉堡.fbx");

    }
}