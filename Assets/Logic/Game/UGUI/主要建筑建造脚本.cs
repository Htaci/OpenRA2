using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class A建造图标 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool is建筑按钮是否打开 = true;
    private bool is建筑是否在生产 = false;
    private float 主要建筑建造耗时 = 12.0f; // 总时间
    private float 建筑已经耗时 = 0.0f; // 已经过去的时间
    public Slider progressBar;
    public TextMeshProUGUI 加载百分比;

    public Image image; // 生产图标
    
    private bool 鼠标是否在图标内 = false;
    private bool is暂停生产 = false;
    private bool is生产完成= false;

    //public GameObject 就绪按钮;
    
    // 当鼠标进入时
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("鼠标进入按钮");
        鼠标是否在图标内 = true;
    }

    // 当鼠标离开时
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("鼠标离开按钮");
        鼠标是否在图标内 = false;
    }

    void Start()
    {
        // 初始化进度条
        if (progressBar == null)
        {
            GameObject gridObject = GameObject.Find("生产进度条");
            progressBar = gridObject.GetComponent<Slider>();
        }
        //progressBar.value = 0.0f;
        加载百分比.text = string.Empty;
    }

    // 激活时
    void OnEnable()
    {

    }
    // 禁用时
    void OnDisable()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonUp(1) && 鼠标是否在图标内)
        {
            if (!is暂停生产)
            {
                Debug.LogAssertion("暂停建造");
                加载百分比.text = "暂停";
                is建筑是否在生产 = false;
                is暂停生产 = true;
            }
            else
            {
                is暂停生产 = false;
                is建筑是否在生产 = false;
                Debug.LogAssertion("建造取消");
                加载百分比.text = string.Empty;
                建筑已经耗时 = 0;
                image.color = new Color32(255, 255, 255, 255);
                //就绪按钮.SetActive(false);
                is生产完成 = false;
                progressBar.value = 0.0f;
            }
        }
        if (Input.GetMouseButtonUp(0) && 鼠标是否在图标内)
        {
            if (is生产完成)
            {
                建筑物放置 建筑物生成 = FindObjectOfType<建筑物放置>();
                建筑物生成.放置预览建筑("");
            }
            else
            {
                Debug.LogAssertion("继续建造");
                is建筑是否在生产 = true;
                is暂停生产 = false;
                image.color = new Color32(150, 150, 150, 255);
                //就绪按钮.SetActive(true);
            }

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (is建筑是否在生产)
        {
            // 每帧更新已经过去的时间
            建筑已经耗时 += 0.05f;

            // 计算已经过去的时间占总时间的百分比
            float percentage = 建筑已经耗时 / 主要建筑建造耗时;

            // 更新进度条
            progressBar.value = percentage;
            int 百分比Int = Mathf.RoundToInt(percentage * 100);
            加载百分比.text = $"{百分比Int}%";

            // 如果已经过去的时间超过总时间，重置时间
            if (建筑已经耗时 >= 主要建筑建造耗时)
            {
                is建筑是否在生产 = false;
                建筑已经耗时 = 0.0f;
                Debug.Log("建造完成");
                // 可以在这里添加建造完成后的逻辑
                is暂停生产 = true;
                is生产完成 = true;
                加载百分比.text = $"就绪";
                image.color = new Color32(150, 150, 150, 255);
                //就绪按钮.SetActive(true);
            }
        }
    }

    public void 开始建造()
    {
        //Debug.Log("按钮触发");
        
        //is建筑是否在生产 = true;
        //建筑已经耗时 = 0.0f; // 重置已经过去的时间
    }
}