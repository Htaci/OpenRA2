using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionPool : MonoBehaviour
{
    // 生产列表池，负责控制列表呈现的

    private Dictionary<string, GameObject> production;

    private List<float> RequiredTime; // 所需时间
    private List<float> ElapsedTime; // 已用时间
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // 新添一个图标
    public void AddNewIcon(string a)
    {
        if (!GetIconPool(a))
        {

        }
    }

    public bool GetIconPool(string a)
    {
        if (production.ContainsKey(a))
        {
            return true;
        }
        else
        {
            return false; 
        }
    }
}
