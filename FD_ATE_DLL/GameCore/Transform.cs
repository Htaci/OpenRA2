// 定义一个表示位置的结构体
using System.Numerics;

public struct Transform
{
    public Vector3 position { get; set; } = new Vector3(0, 0, 0);
    public Quaternion rotation { get; set; } = new Quaternion(0, 0, 0, 1);
    public Vector3 scale { get; set; } = new Vector3(1, 1, 1);

    public Transform(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}

// 定义一个表示四元数的结构体
public struct Quaternion
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float W { get; set; }

    public Quaternion(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }
}