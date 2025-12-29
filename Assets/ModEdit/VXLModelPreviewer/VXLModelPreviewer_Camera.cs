using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VXLModelPreviewer_Camera : MonoBehaviour
{
    [Header("摄像机设置")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float panSpeed = 1f;
    [SerializeField] private float scrollSpeed = 2f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 100f;
    
    [Header("初始设置")]
    [SerializeField] private Vector3 initialPosition = new Vector3(0, 5, -10);
    [SerializeField] private Vector3 initialTarget = Vector3.zero;
    
    [Header("轴线设置")]
    [SerializeField] private bool showAxes = true;
    [SerializeField] private float axisLength = 10f;
    
    [Header("方向图标设置")]
    [SerializeField] private bool showDirectionIcon = true;
    [SerializeField] private float iconOffset = 80f;
    [SerializeField] private float iconAxisLength = 60f;

    private Vector3 targetPosition;
    private float currentDistance;
    private Vector2 lastMousePosition;
    
    private Camera cam;
    private Material lineMaterial;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetPosition = initialTarget;
        currentDistance = Vector3.Distance(transform.position, targetPosition);
        
        CreateLineMaterial();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // 鼠标中键 - 围绕中心点旋转
        if (Input.GetMouseButton(2))
        {
            if (Input.GetMouseButtonDown(2))
            {
                lastMousePosition = Input.mousePosition;
            }
            else
            {
                Vector2 mouseDelta = (Vector2)Input.mousePosition - lastMousePosition;
                RotateAroundTarget(mouseDelta);
                lastMousePosition = Input.mousePosition;
            }
        }
        
        // 鼠标右键 - 平移摄像机和中心点
        if (Input.GetMouseButton(1))
        {
            if (Input.GetMouseButtonDown(1))
            {
                lastMousePosition = Input.mousePosition;
            }
            else
            {
                Vector2 mouseDelta = (Vector2)Input.mousePosition - lastMousePosition;
                PanCamera(mouseDelta);
                lastMousePosition = Input.mousePosition;
            }
        }
        
        // 鼠标滚轮 - 缩放距离
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            ZoomCamera(scroll);
        }
    }

    void RotateAroundTarget(Vector2 mouseDelta)
    {
        // 水平旋转（绕世界Y轴）- 鼠标向左移动，物体向左转
        float horizontalAngle = -mouseDelta.x * rotationSpeed * Time.deltaTime;
        
        // 垂直旋转（绕摄像机局部X轴）- 鼠标向上移动，物体向上转
        float verticalAngle = mouseDelta.y * rotationSpeed * Time.deltaTime;
        
        // 获取当前摄像机相对于目标点的方向
        Vector3 direction = transform.position - targetPosition;
        
        // 水平旋转（绕世界Y轴）
        Quaternion horizontalRotation = Quaternion.AngleAxis(horizontalAngle, Vector3.up);
        direction = horizontalRotation * direction;
        
        // 垂直旋转（绕局部X轴）
        Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;
        Quaternion verticalRotation = Quaternion.AngleAxis(verticalAngle, right);
        direction = verticalRotation * direction;
        
        // 更新摄像机位置和距离
        currentDistance = direction.magnitude;
        transform.position = targetPosition + direction;
        
        // 始终看向目标点
        transform.LookAt(targetPosition);
    }

    void PanCamera(Vector2 mouseDelta)
    {
        // 根据摄像机的右方向和上方向进行平移
        Vector3 right = transform.right;
        Vector3 up = transform.up;
        
        // 计算平移量 - 鼠标向左移动，物体向左；鼠标向上移动，物体向上
        float speedFactor = panSpeed * currentDistance * 0.001f;
        Vector3 panOffset = (-right * mouseDelta.x - up * mouseDelta.y) * speedFactor;
        
        // 同时移动摄像机和目标点
        transform.position += panOffset;
        targetPosition += panOffset;
    }

    void ZoomCamera(float scrollDelta)
    {
        // 调整距离
        currentDistance -= scrollDelta * scrollSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        
        // 保持方向，只改变距离
        Vector3 direction = (transform.position - targetPosition).normalized;
        transform.position = targetPosition + direction * currentDistance;
    }

    /// <summary>
    /// 重置摄像机到初始位置
    /// </summary>
    public void ResetCamera()
    {
        transform.position = initialPosition;
        targetPosition = initialTarget;
        currentDistance = Vector3.Distance(transform.position, targetPosition);
        transform.LookAt(targetPosition);
    }

    void OnRenderObject()
    {
        if (!lineMaterial)
            return;
            
        if (showAxes)
        {
            DrawWorldAxes();
        }
        
        if (showDirectionIcon)
        {
            DrawDirectionIcon();
        }
    }

    void DrawWorldAxes()
    {
        lineMaterial.SetPass(0);
        
        GL.PushMatrix();
        GL.MultMatrix(Matrix4x4.identity);
        GL.Begin(GL.LINES);
        
        // X轴 - 红色
        GL.Color(Color.red);
        GL.Vertex3(-axisLength, 0, 0);
        GL.Vertex3(axisLength, 0, 0);
        
        // Y轴 - 绿色
        GL.Color(Color.green);
        GL.Vertex3(0, -axisLength, 0);
        GL.Vertex3(0, axisLength, 0);
        
        // Z轴 - 蓝色
        GL.Color(Color.blue);
        GL.Vertex3(0, 0, -axisLength);
        GL.Vertex3(0, 0, axisLength);
        
        GL.End();
        GL.PopMatrix();
    }

    void DrawDirectionIcon()
    {
        lineMaterial.SetPass(0);
        
        GL.PushMatrix();
        GL.LoadIdentity();
        GL.LoadProjectionMatrix(GL.GetGPUProjectionMatrix(Matrix4x4.identity, false));
        
        GL.Begin(GL.LINES);
        
        // 计算图标在屏幕空间的中心位置（归一化设备坐标）
        float centerX = 1.0f - iconOffset / Screen.width * 2.0f;
        float centerY = 1.0f - iconOffset / Screen.height * 2.0f;
        float depth = 0f;
        
        // 计算轴的长度（在归一化设备坐标中）
        float axisLengthX = iconAxisLength / Screen.width * 2.0f;
        float axisLengthY = iconAxisLength / Screen.height * 2.0f;
        
        // 获取摄像机的旋转矩阵，用于将世界轴方向转换为视图方向
        Matrix4x4 viewMatrix = cam.worldToCameraMatrix;
        
        // 世界坐标系的三个轴
        Vector3 worldRight = Vector3.right;
        Vector3 worldUp = Vector3.up;
        Vector3 worldForward = Vector3.forward;
        
        // 转换到视图空间
        Vector3 viewRight = viewMatrix.MultiplyVector(worldRight);
        Vector3 viewUp = viewMatrix.MultiplyVector(worldUp);
        Vector3 viewForward = viewMatrix.MultiplyVector(worldForward);
        
        // X轴 - 红色
        GL.Color(Color.red);
        GL.Vertex3(centerX, centerY, depth);
        GL.Vertex3(centerX + viewRight.x * axisLengthX, centerY + viewRight.y * axisLengthY, depth);
        
        // Y轴 - 绿色
        GL.Color(Color.green);
        GL.Vertex3(centerX, centerY, depth);
        GL.Vertex3(centerX + viewUp.x * axisLengthX, centerY + viewUp.y * axisLengthY, depth);
        
        // Z轴 - 蓝色
        GL.Color(Color.blue);
        GL.Vertex3(centerX, centerY, depth);
        GL.Vertex3(centerX + viewForward.x * axisLengthX, centerY + viewForward.y * axisLengthY, depth);
        
        GL.End();
        GL.PopMatrix();
    }

    void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity内置的用于GL绘制的Shader
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    void OnDestroy()
    {
        if (lineMaterial)
        {
            DestroyImmediate(lineMaterial);
        }
    }
}
