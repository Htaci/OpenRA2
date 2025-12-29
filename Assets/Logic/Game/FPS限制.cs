using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System;
using Debug = UnityEngine.Debug;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class FPS限制 : MonoBehaviour
{
    public int FPS = 60;
    public TextMeshProUGUI FPS帧率显示;
    public Text 内存占用显示;

    public bool isFpsUpdate = true;
    public int FPS已显示帧率 = 0;

    public string 内存占用;

    void Start()
    {

        Application.targetFrameRate = FPS;


        StartCoroutine(Tick());
    }

    void Update()
    {
        FPS已显示帧率++;
    }

    private void FixedUpdate()
    {
        long totalMemory = Profiler.GetTotalAllocatedMemoryLong();
        //Debug.Log("程序使用内存: " + totalMemory/1024/1024 + " MB");
        内存占用 = totalMemory / 1024 / 1024 +"MB";
    }

    private IEnumerator Tick()
    {


        while(isFpsUpdate)
        {
            FPS帧率显示.text = $"FPS:{FPS已显示帧率}";
            内存占用显示.text = $"内存占用:{内存占用}";
            FPS已显示帧率 = 0;
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }

    public static string GetMemory()
    {
        Process proc = Process.GetCurrentProcess();
        long b = proc.PrivateMemorySize64;
        for (int i = 0; i < 2; i++)
        {
            b /= 1024;
        }
        return b + "MB";
    }

}
