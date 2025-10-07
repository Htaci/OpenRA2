using Godot;
using System;

/// <summary>
/// 这个类用于管理顶部菜单栏中的项目按钮
/// </summary>
public partial class ProjectButton : Node
{
    [Export]
    public Button projectButton; // 项目按钮节点

    private PopupMenu projectMenu; // 项目菜单节点

    public override void _Ready()
    {
        // 连接按钮的按下信号到相应的方法
        if (projectButton != null)
        {
            GD.Print("找到了 ProjectButton 节点.");
            projectButton.Pressed += OnProjectButtonPressed;
        }
        else
        {
            GD.PrintErr("未找到 ProjectButton 节点，请检查场景树结构。");
        }

        // 创建 PopupMenu
        projectMenu = new PopupMenu();
        // 设置位置为按钮的下方40像素（顶部菜单栏的大小是固定的40像素）
        projectMenu.Position = new Vector2I((int)projectButton.Position.X, (int)(projectButton.Position.Y + 40));
        AddChild(projectMenu);

        // 添加菜单项
        projectMenu.AddItem("选项一", 0);
        projectMenu.AddItem("选项二", 1);
        projectMenu.AddItem("选项三", 2);

        // 连接菜单项点击信号
        projectMenu.IdPressed += OnMenuItemPressed;

        GD.Print("ProjectButton 绑定事件完成");
    }

    private void OnProjectButtonPressed()
    {
        GD.Print("Project Button Pressed");
        // 打开 项目菜单
        if (projectMenu != null)
            projectMenu.Popup();
    }

    
    private void OnMenuItemPressed(long id)
    {
        GD.Print($"你点击了菜单项：{id}");
        // 根据 id 做不同的操作
    }
}
