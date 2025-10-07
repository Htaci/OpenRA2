using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

public partial class GameLogicManager : IDisposable
{
    // 逻辑帧更新频率（ms/毫秒）
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(50));
    private readonly Task _loopTask;
    private volatile bool _running = true;


    #region 统计信息
    private int _TPS; // 每秒逻辑更新帧数
    private Stopwatch _mspt; // 每帧耗时
    // 工作集内存占用（MB）
    public double mb;
    
    #endregion

    // 测试用
    int 已过时间 = 0;

    /// <summary>
    /// 游戏逻辑管理类的构造函数
    /// 用于初始化游戏逻辑管理器并启动游戏主循环
    /// </summary>
    public GameLogicManager()
    {
        _loopTask = Task.Run(RunLoop); // 单线程异步循环，启动游戏主循环任务
    }

    private async Task RunLoop()
    {
        while (await _timer.WaitForNextTickAsync()) // 每 50 ms 来一次
        {
            Update(); // 同步调用，保证不重叠
            

            if (_mspt.ElapsedMilliseconds >= 50)
                _TPS = (int)(1000 / _mspt.ElapsedMilliseconds);
            else
                _TPS = 20;


            mb = Process.GetCurrentProcess().WorkingSet64 / (1024.0 * 1024.0);
            // 回到上一行开头，覆盖掉旧内容
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
            // 输出调试参数
            Console.WriteLine($"MSPT: {_mspt.ElapsedMilliseconds} ms，TPS: {_TPS}，工作集（物理内存）= {mb:F1} MB，已过时间: {已过时间/20} 秒");

        }
    }

    // 帧逻辑
    private void Update()
    {
        // 统计用:开始计时
        _mspt = Stopwatch.StartNew();

        已过时间++;

        // 模拟耗时
        //Thread.Sleep(40); // 故意让它超过 50 ms，观察是否重叠

        // 结束计时
        _mspt.Stop();
    }

    public void Dispose()
    {
        _timer.Dispose();
        _running = false;
        _loopTask.Wait();
    }

    // 下面为逻辑部分 // 

    #region 生产更新
    //private void 

    /// <summary>
    /// 玩家层
    /// </summary>
    public class PlayerGroup
    {
        public List<ProductionAgreement> ProductionsList { get; set; } = new List<ProductionAgreement>();
        public int PlayerUID { get; set; }
    }
    /// <summary>
    /// 单条生产列队
    /// </summary>
    public class ProductionAgreement
    {
        public AProduction ProductionA { get; set; } = new AProduction();
        public BProduction ProductionB { get; set; } = new BProduction();
        public CProduction ProductionC { get; set; } = new CProduction();
        public DProduction ProductionD { get; set; } = new DProduction();
    }

    public class AProduction // 主要建筑栏位
    {
        public UnitProduction Building { get; set; } = null;// 当前生产的建筑单位
        public bool IsProduction { get; set; } = false;// 该生产栏位是否正在使用
        public bool IsSuspend { get; set; } = false;// 暂停生产

        public List<string> AvailableList { get; set; } = new List<string>(); // 可用生产列表
    }

    public class BProduction // 防御建筑栏位
    {
        public UnitProduction Building { get; set; } = null;// 当前生产的建筑单位
        public bool IsProduction { get; set; } = false;// 该生产栏位是否正在使用
        public bool IsSuspend { get; set; } = false;// 暂停生产
                                                    //public List<UnitProduction> unitProductions { get; set; } // 顺序生产列队

        public List<string> AvailableList { get; set; } = new List<string>(); // 可用生产列表
    }

    public class CProduction // 步兵单位栏位
    {
        public bool IsSuspend { get; set; } // 暂停生产
        public List<UnitProduction> unitProductions { get; set; } = new List<UnitProduction>();// 顺序生产列队

        public List<string> AvailableList { get; set; } = new List<string>(); // 可用生产列表
    }

    public class DProduction // 装甲单位栏位
    {
        public List<UnitProduction> unitProductionsA { get; set; } = new List<UnitProduction>();// 顺序生产列队(陆军/直升机)
        public bool IsSuspendA { get; set; } // 暂停生产
        public List<UnitProduction> unitProductionsB { get; set; } = new List<UnitProduction>();// 顺序生产列队(大型陆军单位)
        public bool IsSuspendB { get; set; }
        public List<UnitProduction> unitProductionsC { get; set; } = new List<UnitProduction>();// 顺序生产列队(战机)
        public bool IsSuspendC { get; set; }
        public List<UnitProduction> unitProductionsD { get; set; } = new List<UnitProduction>();// 顺序生产列队(海军)
        public bool IsSuspendD { get; set; }

        public List<string> AvailableList { get; set; } = new List<string>(); // 可用生产列表
    }

    /// <summary>
    /// 单个生产单位
    /// </summary>
    public class UnitProduction
    {
        public string UnitId { get; set; } // 建筑/单位唯一码
        public float Schedule { get; set; } = 0;// 当前生产已用时间
        public float TotalTime { get; set; } // 生产所需时间
        public bool IsProduction { get; set; } // 是否完成
    }

    #endregion
}
