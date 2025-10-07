using Godot;
using System;

public partial class ButtonClick : Node
{
    [Export]
    public Button button1; // 按钮节点
    [Export]
    public Button button2; // 按钮节点
    [Export]
    public Button button3; // 按钮节点

    public override void _Ready()
    {
        // 连接按钮的按下信号到相应的方法
        if(button1 != null)
            button1.Pressed += OnButton1Pressed;

        if(button2 != null)
            button2.Pressed += OnButton2Pressed;

        if(button3 != null)
            button3.Pressed += OnButtonJumpModEdit;

        GD.Print("Button绑定事件完成");
    }

    private void OnButton1Pressed()
    {
        GD.Print("Button 1 Pressed");
        // 在这里添加按钮1按下时的逻辑
    }

    private void OnButton2Pressed()
    {
        GD.Print("Button 2 Pressed");
        // 在这里添加按钮2按下时的逻辑
    }


    /// <summary>
    /// 跳转到Mod编辑器
    /// </summary>
    private void OnButtonJumpModEdit()
    {
        GD.Print("Button 3 Pressed");
        // 跳转到Mod编辑器场景
        GetTree().ChangeSceneToFile("Scenes/ModEdit.tscn");
        DisplayServer.WindowSetTitle("Ra2ModEditor - 无项目");
    }

}
