using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LogicMain
{
    const double FixedDt = 1.0 / 20;   // 更新频率
    double accumulator = 0;
    DateTime last = DateTime.UtcNow;

    // 逻辑入口
    public void GameStart()
    {
        // 创建一个线程
        Thread updateThread = new Thread(FixedUpdate);
        
        // 启动线程
        updateThread.Start();
    }

    // 固定逻辑帧循环
    void FixedUpdate()
    {
        while (true)
        {
            DateTime now = DateTime.UtcNow;
            double frameTime = (now - last).TotalSeconds;
            last = now;

            frameTime = Math.Min(frameTime, 0.25); // 限制最大帧时间
            accumulator += frameTime;

            while (accumulator >= FixedDt)
            {
                GameUpdate();         // 调用游戏逻辑更新
                accumulator -= FixedDt;
            }

            Thread.Sleep(1);           // 防止 100% CPU
        }
    }

    // 逻辑更新
    public void GameUpdate()
    {
        // 更新建筑生产
        Production.UpdateProduction();
    }
}
