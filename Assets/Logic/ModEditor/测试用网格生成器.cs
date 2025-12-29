using UnityEngine;

public class 网格生成器 : MonoBehaviour
{
    // 网格的宽度和长度
    public int width = 5;  // 横向格子数
    public int length = 5; // 纵向格子数
    public GameObject squarePrefab; // 网格预制件

    void Start()
    {
        GenerateIsometricGrid();
    }

    void GenerateIsometricGrid()
    {
        // 遍历所有行
        for (int z = 0; z < length; z++)
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
        for (int z = 0; z < length-1; z++)
        {
            // 遍历每一列
            for (int x = 0; x < width-1; x++)
            {
                // 计算等距网格的 x, z 坐标
                // 正确的等距坐标计算公式
                float isoX = (x - z) * 1f;  // x 方向的偏移
                float isoZ = (x + z) * 1f;  // z 方向的偏移

                // 创建网格实例
                Vector3 position = new Vector3(isoX, 0f, isoZ+1);
                GameObject newSquare = Instantiate(squarePrefab, position, Quaternion.identity);

                // 将新生成的网格作为当前脚本所在物体的子物体，方便管理
                newSquare.transform.parent = transform;

                // 不需要额外的旋转，网格已经是平行的
                newSquare.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
    }
}
