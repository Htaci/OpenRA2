using UnityEngine;
using UnityEngine.UI;

// 自定义形状的按钮图形
public class ImagePlus : Image
{
    PolygonCollider2D imageCollider;

    protected override void Awake()
    {
        base.Awake();
        imageCollider = GetComponent<PolygonCollider2D>();
    }

    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        //return base.IsRaycastLocationValid(screenPoint, eventCamera);
        var _hit = imageCollider.OverlapPoint(screenPoint);
        return _hit;
    }
}
