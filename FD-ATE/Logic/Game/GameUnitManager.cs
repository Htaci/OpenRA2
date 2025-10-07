using Godot;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Loader;    


[GlobalClass]
public partial class GameUnitManager : Node3D
{
    public static GameUnitManager Instance { get; private set; }

    private AssemblyLoadContext _assemblyContext;
    public override void _Ready()
    {
        Instance = this;
        GD.Print("GameUnitManager initialized!");
    }

        // 创建游戏对象的实例，现在用于测试反射
    // 创建游戏对象节点，并初始化 ObjectType
    public static void CreateInstance(object obj)
    {
        // 获取对象类型
        Type objType = obj.GetType();
        GD.Print($"尝试创建 GameUnit 节点，类型为: {objType}");

        // 创建 GameUnit 节点
        var gameUnit = new GameUnit();
        gameUnit.UnitObject = obj;

        // 添加节点到场景树
        // 判断是否在主线程 使用保存的 splash_screen.MainThreadId 来判断
        if (Thread.CurrentThread.ManagedThreadId == splash_screen.MainThreadId)
        {
            // 主线程，直接添加
            Instance.AddChild(gameUnit);
            GD.Print("在主线程中添加 GameUnit 节点");
        }
        else
        {
            // 非主线程，使用 CallDeferred 添加节点
            Instance.CallDeferred("add_child", gameUnit);
            GD.Print("在非主线程中使用 CallDeferred 添加 GameUnit 节点");
        }
 
    }
}
