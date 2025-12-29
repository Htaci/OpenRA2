using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 建筑物放置预览
/// </summary>
public class BuildingPlacementPreview : MonoBehaviour
{
    // 目标建筑的渲染器
    private Renderer buildingRenderer;
    // 预览的材质
    private Material lineMaterial;


    /// <summary>
    /// 检查是否可以放置建筑物
    /// </summary>
    public void CheckBuildingPlacement()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        buildingRenderer = gameObject.GetComponent<Renderer>();
        // 创建材质
        lineMaterial = new Material(Shader.Find("Knife/Hologram Shader Unlit Depth Mask"));
        

        if (lineMaterial != null)
        {
            lineMaterial.SetColor("_MainColor", new Color(1, 0, 1,0.21f)); // 设置 Shader 的颜色参数为紫色
        }

        // 启用 Fresnel 特效的宏定义
        lineMaterial.EnableKeyword("_FRESNELFEATURE_ON");
        // 启用随机闪烁特效的宏定义
        //lineMaterial.EnableKeyword("_RANDOMGLITCHFEATURE_ON");
        

        // 将材质赋予给游戏对象
        buildingRenderer.material = lineMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }
}
