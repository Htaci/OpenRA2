using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 三维瓦片地图编辑器 : MonoBehaviour
{
    public GameObject squarePrefab;
    private GameObject currentSquare;

    private void Start()
    {
        currentSquare = Instantiate(squarePrefab);
        //currentSquare.transform.localScale = new Vector3(1, 1, 1);
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        // 四舍五入到最近的整数
        Vector3 gridPosition = new Vector3(Mathf.Round(mouseWorldPosition.x), 0, Mathf.Round(mouseWorldPosition.z));
        currentSquare.transform.position = gridPosition;

        // 检查是否是鼠标左键抬起
        if (Input.GetMouseButtonUp(0))
        {
            // 实例化一个新的瓦片对象
            GameObject newSquare = Instantiate(squarePrefab, gridPosition, Quaternion.identity);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
        {
            return hitInfo.point;
        }
        
        return Vector3.zero;
    }
}