using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class 调试类方法 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void 内存占用()
    {
        long totalAllocatedMemory = System.GC.GetTotalMemory(false);

        Debug.Log("分配总内存: " + totalAllocatedMemory/ (1024.0f * 1024.0f) + "Mb");
    }
}
