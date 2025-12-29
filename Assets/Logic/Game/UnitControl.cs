using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

public class UnitControl: MonoBehaviour
{
    public HashSet<GameObject> gameObjects = new HashSet<GameObject>();
    public GameObject 游戏对象集合;
    //public Material selectedMat; // 材质
    private Material selectedMat;
    // 框选填充：是否绘制白色填充
    public bool drawFill = true;
    private Vector3 startPosition; // 开始按下的点
    private Vector3 endPosition; // 结束点
    public static bool isSelect; // 绘制状态
    private float fadeAlpha = 0.2f;  // 填充初始透明度
    private float LinesAlpha = 1f; // 线条初始透明度
    private float fadeSpeed = 0.8f;  // 透明度变化速度

    // 相机
    public float moveSpeed = 5f; // 相机移动速度

    // 建筑物放置
    public GameObject currentSquare; // 当前的空对象，用于显示建筑物
    public bool is是否放置状态 = false; // 是否处于放置状态

    // 鼠标左键事件
    private MouseInput inputActions;
    void Awake()
    {
        // 创建材质
        selectedMat = new Material(Shader.Find("Sprites/Default"));
        //selectedMat = new Material(Shader.Find("UI/SelectionBox"));
        inputActions = new MouseInput();
    }

    void OnEnable()
    {
        // 启用输入系统
        inputActions.MouseActions.Enable();

        // 绑定事件
        inputActions.MouseActions.LeftClick.started += OnLeftClickDown;
        //inputActions.MouseActions.LeftClick.performed += OnLeftClickHeld; // 长按/拖动
        inputActions.MouseActions.LeftClick.canceled += OnLeftClickUp;
    }

    void OnDisable()
    {
        // 解绑事件
        inputActions.MouseActions.LeftClick.started -= OnLeftClickDown;
        //inputActions.MouseActions.LeftClick.performed -= OnLeftClickHeld;
        inputActions.MouseActions.LeftClick.canceled -= OnLeftClickUp;

        // 禁用输入系统
        inputActions.MouseActions.Disable();
    }

    // 鼠标左键按下
    private void OnLeftClickDown(InputAction.CallbackContext context)
    {
        //Debug.Log("鼠标被按下，位置: " + Input.mousePosition);
        startPosition = Input.mousePosition;
        endPosition = Input.mousePosition;
        isSelect = true;
        fadeAlpha = 0.2f;  // 重置透明度
        LinesAlpha = 1f;
    }

    // 鼠标左键抬起
    private void OnLeftClickUp(InputAction.CallbackContext context)
    {
        //Debug.Log("鼠标被抬起，位置: " + Input.mousePosition);
        if (isSelect)
        {
            isSelect = false;
            // 判断被框选单位
            OnBoxSelect();
        }
    }


    void Update()
    {
        // 命令选中单位移动到鼠标位置
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 vector2a = GetMouseWorldPosition();
            Vector3 gridPositiona = new Vector3(Mathf.Round(vector2a.x), 0, Mathf.Round(vector2a.z));
            Vector2Int targetPos = new Vector2Int((int)gridPositiona.x, (int)gridPositiona.z);
            // 遍历选中列表中的每个对象
            foreach (GameObject gameObject in gameObjects)
            {
                // 获取对象上的CharacterMovement脚本
                CharacterMovement characterMovement = gameObject.GetComponent<CharacterMovement>();
                if (characterMovement != null)
                {
                    // 调用jps方法
                    Vector3 vector2b = gameObject.transform.position;
                    Vector3 gridPositionb = new Vector3(Mathf.Round(vector2b.x), 0, Mathf.Round(vector2b.z));
                    Vector2Int currentPos = new Vector2Int((int)gridPositionb.x, (int)gridPositionb.z);
                    characterMovement.JpsPathfinding(currentPos, targetPos);
                    characterMovement.isMoving = true;
                }
                else
                {
                    Debug.LogError($"CharacterMovement脚本未挂在对象 {gameObject.name} 上！");
                }
            }
        }


        //if (Input.GetMouseButtonDown(0))
        //{
        //    startPosition = Input.mousePosition;
        //    endPosition = Input.mousePosition;
        //    isSelect = true;
        //    fadeAlpha = 0.2f;  // 重置透明度
        //    LinesAlpha = 1f;
        //}
        //else
        //{
        //    if (Input.GetMouseButton(0))
        //    {
        //        endPosition = Input.mousePosition;
        //    }

        //    if (Input.GetMouseButtonUp(0) && isSelect)
        //    {
        //        isSelect = false;
        //        // 判断被框选单位
        //        OnBoxSelect();
        //    }
        //}

        // 更新移动时鼠标的位置
        if (isSelect)
        {
            endPosition = Input.mousePosition;
        }


        if (Input.GetMouseButtonUp(1))
        {
            Debug.Log("取消选中");
            gameObjects.Clear();
        }

        // 如果选框已经完成绘制，开始渐变消失
        if (!isSelect && fadeAlpha > 0)
        {
            fadeAlpha -= fadeSpeed * Time.deltaTime;  // 填充渐变透明度
            LinesAlpha -= fadeSpeed * 5 * Time.deltaTime; // 线条渐变透明度
        }
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Debug.Log("鼠标位置: " + Input.mousePosition);
        //}
        //鼠标对象.transform.position = Input.mousePosition;

    }

    // 框选矩形绘制
    private void OnPostRender()
    {
        if (isSelect || fadeAlpha > 0)  // 当选框处于激活状态，或者透明度大于0时绘制
        {
            Vector3Int p1 = Vector3Int.RoundToInt(startPosition);
            Vector3Int p2 = Vector3Int.RoundToInt(startPosition + Vector3.Project(endPosition - startPosition, Vector3.right));  // 右下角;
            Vector3Int p3 = Vector3Int.RoundToInt(endPosition);
            Vector3Int p4 = Vector3Int.RoundToInt(startPosition + Vector3.Project(endPosition - startPosition, Vector3.up));    // 左上角

            selectedMat.SetPass(0);
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            if (drawFill)
            {
                // 填充颜色，透明度随 fadeAlpha 变化
                Color fillColor = Color.white;
                fillColor.a = fadeAlpha;  // 透明度随 fadeAlpha 改变

                GL.Begin(GL.QUADS);
                GL.Color(fillColor);

                //GL.Vertex(startPosition);  // 左下角
                GL.Vertex(p1);  // 左下角
                                //Vector3 p2 = startPosition + Vector3.Project(endPosition - startPosition, Vector3.right);  // 右下角
                GL.Vertex(p2);  // 右下角
                                //GL.Vertex(endPosition);  // 右上角
                GL.Vertex(p3);  // 右上角
                                //Vector3 p4 = startPosition + Vector3.Project(endPosition - startPosition, Vector3.up);  // 左上角
                GL.Vertex(p4);  // 左上角
                GL.End();
            }
            // 绘制边缘的粗线
            Color edgeColor = Color.red;
            edgeColor.a = LinesAlpha;  // 边缘渐变
            GL.Begin(GL.LINES);
            GL.Color(edgeColor);


            GL.Vertex(p1);  // 左下角startPosition
            GL.Vertex(p2);  // 右下角

            GL.Vertex(p2);  // 右下角
            GL.Vertex(p3);  // 右上角endPosition

            GL.Vertex(p3);  // 右上角endPosition
            GL.Vertex(p4);  // 左上角

            GL.Vertex(p4);  // 左上角
            GL.Vertex(p1);  // 左下角startPosition

            // test



            // test

            GL.End();

            GL.PopMatrix();
        }
    }


    private void OnBoxSelect()
    {
        // 计算框选区域的屏幕坐标范围
        float minX = Mathf.Min(startPosition.x, endPosition.x);
        float maxX = Mathf.Max(startPosition.x, endPosition.x);
        float minY = Mathf.Min(startPosition.y, endPosition.y);
        float maxY = Mathf.Max(startPosition.y, endPosition.y);

        //Debug.Log("开始的点：" + startPosition + "\n结束的点：" + endPosition);
        //Debug.Log("minX:"+minX+"  maxX:"+maxX+"  minY:"+minY+"  maxY:"+maxY);

        // 获取游戏对象集合下的所有子对象
        int childCount = 游戏对象集合.transform.childCount;

        // 遍历子对象
        for (int i = 0; i < childCount; i++)
        {
            // 获取直接子对象的Transform
            Transform childTransform = 游戏对象集合.transform.GetChild(i);

            // 获取直接子对象的世界坐标
            Vector3 worldPos = childTransform.position;

            // 将世界坐标转换为屏幕坐标
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            // 输出直接子对象的名称和屏幕坐标
            //Debug.Log(childTransform.name + "的屏幕坐标是：" + screenPos);
            if (screenPos.x > minX && screenPos.x < maxX && screenPos.y > minY && screenPos.y < maxY)
            {
                if (!gameObjects.Contains(childTransform.gameObject))
                {
                    gameObjects.Add(childTransform.gameObject);
                    Debug.Log("选中了：" + childTransform.name);
                }
                
            }
        }
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


}
