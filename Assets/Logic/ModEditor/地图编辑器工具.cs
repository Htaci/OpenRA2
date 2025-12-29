using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class 地图编辑器工具 : MonoBehaviour
{
    public GameObject squarePrefab; // 地图瓦片预制件
    // 当前打开的地图
    public MapInfo mapInfo;
    void Start()
    {
        // 假设这个BlockInfo是方法传入的参数，这里用于测试,单个区块可以有12*12个格子
        BlockInfo blockInfo1 = new BlockInfo();

        TileInfo a1 = new TileInfo
        {
            tilePosition = new Vector3(0, 0, 0),
            tileSlope = 0,
        };
        TileInfo a2 = new TileInfo
        {
            tilePosition = new Vector3(1, 0, 0),
            tileSlope = 0,
        };
        TileInfo a3 = new TileInfo
        {
            tilePosition = new Vector3(2, 0, 0),
            tileSlope = 0,
        };
        TileInfo a4 = new TileInfo
        {
            tilePosition = new Vector3(3, 0, 0),
            tileSlope = 0,
        };
        TileInfo a5 = new TileInfo
        {
            tilePosition = new Vector3(4, 0, 0),
            tileSlope = 0,
        };

        blockInfo1.tileInfo.Add(a1);
        blockInfo1.tileInfo.Add(a2);
        blockInfo1.tileInfo.Add(a3);
        blockInfo1.tileInfo.Add(a4);
        blockInfo1.tileInfo.Add(a5);

        //GridManager.Instance.CreateBlockMesh(blockInfo1);

        //NewMap(12, 12);
    }

    /// <summary>
    /// 创建新地图
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    void NewMap(string mapName,int width,int height)
    {
        // 遍历所有行
        for (int z = 0; z < height; z++)
        {
            // 遍历正常行
            for (int x = 0; x < width; x++)
            {
                // 计算等距网格的 x, z 坐标
                // 正确的等距坐标计算公式
                float isoX = (x - z) * 1f;  // x 方向的偏移
                float isoZ = (x + z) * 1f;  // z 方向的偏移

                // 创建网格实例
                Vector3 position = new Vector3(isoX, 0f, isoZ);
                GameObject newSquare = Instantiate(squarePrefab, position, Quaternion.identity);

                // 将新生成的网格作为当前脚本所在物体的子物体，方便管理
                newSquare.transform.parent = transform;

                // 不需要额外的旋转，网格已经是平行的
                newSquare.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }

        // 遍历中间行
        for (int z = 0; z < height - 1; z++)
        {
            // 遍历每一列
            for (int x = 0; x < width - 1; x++)
            {
                // 计算等距网格的 x, z 坐标
                // 正确的等距坐标计算公式
                float isoX = (x - z) * 1f;  // x 方向的偏移
                float isoZ = (x + z) * 1f;  // z 方向的偏移

                // 创建网格实例
                Vector3 position = new Vector3(isoX, 0f, isoZ + 1);
                GameObject newSquare = Instantiate(squarePrefab, position, Quaternion.identity);

                // 将新生成的网格作为当前脚本所在物体的子物体，方便管理
                newSquare.transform.parent = transform;

                // 不需要额外的旋转，网格已经是平行的
                newSquare.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
    }

}
