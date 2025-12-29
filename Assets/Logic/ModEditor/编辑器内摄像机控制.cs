using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 编辑器内摄像机控制 : MonoBehaviour
{
    public bool NoMove = false; // 相机禁止移动
    public float moveSpeed = 5f; // 默认相机移动速度

    // 屏幕边缘
    //private int OverallHorizontal; // 屏幕总长度
    //private int OverallVertical; // 屏幕总宽度
    //private int HorizontalA;
    //private int HorizontalB;
    //private int VerticalA;
    //private int VerticalB;

    // 右键快速移动视角
    private Vector3 startPosition; // 右键按下的点
    private Vector3 endPosition; // 拖动的点
    private bool FastMove = false; // 右键快速移动视角
    public float baseSpeed = 1.0f; // 基础移动速度
    public float speedMultiplier = 0.1f; // 速度乘数，根据拖动距离调整速度

    // Mac触摸板快速拖动视角
    public float dragSpeed = 1.0f; // 拖动速度
    public float inertiaSpeed = 0.5f; // 惯性滑行速度
    private Vector2 lastTouchPosition;
    private Vector2 inertiaVelocity;

    // 滚轮 摄像头拉近远离

    private Material selectedMat;

    // 控制相机移动的脚本
    void Start()
    {
        selectedMat = new Material(Shader.Find("Sprites/Default"));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            startPosition = Input.mousePosition;
            endPosition = Input.mousePosition;
            NoMove = true;
            FastMove = true;
        }
        else
        {
            if (Input.GetMouseButton(1))
            {
                endPosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(1))
            {
                NoMove = false;
                FastMove = false;
            }
        }

        if (FastMove)
        {
            FastMoveCamera();
        }

        MacCameraMove();

    }
    /// <summary>
    /// Mac 触摸板快速拖动视角
    /// </summary>
    public void MacCameraMove()
    {
        //Debug.Log($"Mac 触摸板快速拖动视角:当前触点数量{Input.touches}");
        // 检测是否有两个触摸点
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);
            Debug.Log("Mac 触摸板快速拖动视角:有两个触点");

            // 检测触摸状态
            if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                Debug.Log("Mac 触摸板快速拖动视角:触摸点移动");
                // 计算两个触摸点的中心位置
                Vector2 touchDeltaPosition = (touch0.deltaPosition + touch1.deltaPosition) / 2.0f;

                // 更新摄像机位置
                transform.position += new Vector3(touchDeltaPosition.x, 0, touchDeltaPosition.y) * dragSpeed;

                // 更新惯性速度
                inertiaVelocity = touchDeltaPosition * dragSpeed;
            }
        }
        else
        {
            // 如果没有触摸，应用惯性滑行
            if (inertiaVelocity.magnitude > 0.01f)
            {
                transform.position += new Vector3(inertiaVelocity.x, 0, inertiaVelocity.y) * inertiaSpeed;
                inertiaVelocity *= 0.95f; // 减少惯性速度
            }
            else
            {
                inertiaVelocity = Vector2.zero;
            }
        }
    }

    private void FastMoveCamera()
    {
        // 计算鼠标拖动的向量（注意这里取反实现拖动方向同步）
        Vector3 dragVector = startPosition - Input.mousePosition;

        // 获取摄像机旋转后的方向
        Quaternion cameraRotation = transform.rotation;
        Vector3 right = cameraRotation * Vector3.right;
        Vector3 up = cameraRotation * Vector3.up;

        // 计算移动速度
        float speed = baseSpeed + dragVector.magnitude * speedMultiplier;

        // 将屏幕拖动方向转换到世界空间
        Vector3 moveDirection = -(right * dragVector.x + up * dragVector.y).normalized;

        // 移动摄像机
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }


    // 框选矩形绘制
    private void OnPostRender()
    {
        if (FastMove)  // 当选框处于激活状态，或者透明度大于0时绘制
        {
            selectedMat.SetPass(0);
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            // 绘制边缘的粗线
            Color edgeColor = Color.white;
            edgeColor.a = 1;
            GL.Begin(GL.LINES);
            GL.Color(edgeColor);

            GL.Vertex(startPosition);  // 左下角
            GL.Vertex(endPosition);  // 右下角

            GL.End();

            GL.PopMatrix();
        }
    }

}
