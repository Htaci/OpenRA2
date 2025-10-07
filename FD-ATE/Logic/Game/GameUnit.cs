using Godot;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Loader;

[GlobalClass]
public partial class GameUnit : Node3D
{
    // 定义一个 object 属性，用于储存游戏对象的实例
    public object UnitObject { get; set; }

    // 初始化函数
    public override void _Ready()
    {

    }

    // 每帧更新函数
    public override void _Process(double delta)
    {
        // 在这里可以添加对 UnitObject 的逻辑处理

    }



}