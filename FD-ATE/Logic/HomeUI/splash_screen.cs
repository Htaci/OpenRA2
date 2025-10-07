using Godot;
using System;
using System.Threading;

public partial class splash_screen : Node
{
    [Export]
    public TextureRect textureRect;     // 启动图片节点
    [Export]
    public TextureRect backgroundTextureRect; // 启动界面背景

    private float alpha = 1.0f; // 初始透明度
    private float fadeSpeed = 1.5f; // 消失速度
    private double fadeTime = 2.0f; // 消失时间
    private bool isFading = false;
    public static int MainThreadId;
    public override void _Ready()
    {
        //DisplayServer.WindowSetTitle("Ra2 - Starting...");
        GD.Print(backgroundTextureRect == null ? "backgroundTextureRect is null" : "backgroundTextureRect is OK");
        // 记录主线程ID
        MainThreadId = Thread.CurrentThread.ManagedThreadId;
        // 在这里执行初始化操作
        // 设置初始透明度
        textureRect.Modulate = new Color(1, 1, 1, alpha);
        StartFadeOutTask();
    }

    // 启动淡出任务
    private async void StartFadeOutTask()
    {
        // 等待淡出前的停留时间
        await System.Threading.Tasks.Task.Delay((int)(fadeTime * 1000));
        isFading = true;
        int frameInterval = 1000 / 60; // 约16ms一帧
        while (alpha > 0)
        {
            alpha -= fadeSpeed * (frameInterval / 1000f);
            if (alpha < 0) alpha = 0;
            // 回到主线程赋值
            CallDeferred(nameof(UpdateAlpha));
            await System.Threading.Tasks.Task.Delay(frameInterval);
        }
        // 回到主线程销毁节点
        CallDeferred(nameof(CleanupNodes));
    }

    // 更新透明度
    private void UpdateAlpha()
    {
        if (textureRect != null)
            textureRect.Modulate = new Color(1, 1, 1, alpha);
        if (backgroundTextureRect != null)
            backgroundTextureRect.Modulate = new Color(1, 1, 1, alpha);
    }

    // 清理节点
    private void CleanupNodes()
    {
        textureRect?.QueueFree();
        backgroundTextureRect?.QueueFree();
    }

    private bool titleSet = false;
    public override void _Process(double delta)
    {
        if (!titleSet)
        {
            DisplayServer.WindowSetTitle("Ra2 - Starting...");
            titleSet = true;
        }
        // ...existing code...
    }
}
