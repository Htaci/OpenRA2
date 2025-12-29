using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class 建筑物放置 : MonoBehaviour
{
    public GameObject currentSquare; // 当前的空对象，用于显示建筑物
    public bool is是否放置状态 = false; // 是否处于放置状态


    

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        

        if (is是否放置状态)
        {
            // 获取鼠标的世界坐标
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            // 四舍五入到最近的整数（保证坐标对齐到网格）
            Vector3 gridPosition = new Vector3(Mathf.Round(mouseWorldPosition.x), -1, Mathf.Round(mouseWorldPosition.z));

            // 如果当前的空对象为空，创建一个空对象并初始化它
            if (currentSquare == null)
            {
                Debug.Log("建筑对象为空");
            }

            // 将 currentSquare 移动到鼠标的位置
            currentSquare.transform.position = gridPosition;
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (currentSquare == null)
            {
                Debug.Log("建筑对象为空");
            }
            Debug.Log("取消放置");
            Destroy(currentSquare);
            is是否放置状态 = false;
        }
    }

    // 放置预览建筑物
    public void 放置预览建筑(string BuildingUid)
    {
        GameObject prefab = Resources.Load<GameObject>("游戏素材/建筑物素材/建筑放置预览/su兵营预览");
        if (prefab != null)
        {
            currentSquare = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("预制体加载失败");
        }

        is是否放置状态 = true; // 开启放置状态
    }

    // 获取鼠标在世界坐标中的位置
    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask mapLayerMask = 1 << 7; // 7:Map 图层的编号
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, mapLayerMask))
        {
            return hitInfo.point;
        }

        return Vector3.zero;
    }

    public void 信息查询(string BuildingUid)
    {

    }

    

    
}
