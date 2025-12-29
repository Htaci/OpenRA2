// 这是一个游戏对象基类，所有的游戏实体都将继承这个基类
using System;
using System.Numerics;
using System.Collections.Generic;

namespace GameCore
{
    public class GameObject
    {
        // 无参数构造函数，必须为无参数，包括继承此类的子类，也必须有无参数构造函数
        // 因为需要通过反射创建对象实例
        public GameObject()
        {
        }

        #region 基本属性
        // Game唯一标识符，只可访问，不可修改
        public Guid Guid { get; set; }
        // 对象名称，允许读写，此属性为在游戏内显示的单位名称，默认"Unnamed"
        public string Name { get; set; } = "Unnamed";
        // 对象描述，允许读写，默认空字符串
        public string Description { get; set; } = string.Empty;
        // 对象ID名称，允许读写，此属性为程序内使用的单位标识符，默认"undefined"
        public string IDName { get; set; } = "undefined";


        #endregion

        #region 生命属性
        // 对象生命值，不允许直接访问，默认100
        private int health = 100;

        // 只读属性，外部只能访问不能直接修改
        public int Health => health;

        /// <summary>
        /// 扣除生命值
        /// </summary>
        /// <param name="amount"></param>
        public void TakeDamage(int amount)
        {
            health -= amount;
            if (health < 0) health = 0;
        }
        /// <summary>
        /// 修改生命值
        /// </summary>
        /// <param name="amount"></param>
        public void SetHealth(int amount)
        {
            health = amount < 0 ? 0 : amount;
        }

        #endregion

        #region 护甲属性
        // 护甲类型，默认无护甲
        public ArmorType Armor { get; set; } = ArmorType.Get("None") ?? new ArmorType("None");

        #endregion

        #region 位置属性
        // 是否允许移动，属于单位属性，用于区分作战单位与建筑，默认允许，注意，不推荐动态修改此属性
        public bool CanMove { get; set; } = true;
        // 单位是否暂停移动，属于状态属性，默认不暂停，如果需要控制单位移动，请通过此属性，而非CanMove
        public bool IsMovementPaused { get; set; } = false;
        // 单位是否允许移动
        public bool IsMoving { get; set; } = false;
        // 移动速度，单位每秒移动的距离，默认5.0f
        public float MoveSpeed { get; set; } = 5.0f;

        // 位置属性，允许读写，默认(0,0,0)
        public Vector3 position { get; set; } = new Vector3(0, 0, 0);
        // 旋转属性，允许读写，默认(0,0,0,1)
        public Quaternion rotation { get; set; } = new Quaternion(0, 0, 0, 1);
        // 缩放属性，允许读写，默认(1,1,1)
        public Vector3 scale { get; set; } = new Vector3(1, 1, 1);

        // 寻路路径，因为地图是二维的，所以路径点是二维的，默认空列表
        public List<Vector2Int> path { get; set; } = new List<Vector2Int>();
        // 当前路径节点索引，不建议新手随意修改，一定会导致寻路问题
        public int pathIndex = 0;
        // 下一个方块位置，此属性指当前单位所在的格子的下一个格子位置，为非节点位置，修改一定会导致问题
        // 解释：按理说，jps返回的是跳点路径，但游戏中的场景是多变的，因此需要每走一格前就需要判断前方是否可以走，如果发现不能走需要重新寻路
        public Vector2Int nextPosition { get; set; } = new Vector2Int(0, 0);
        // 下一个节点位置，不建议小白修改，如果想更改路径请使用别的方式
        public Vector2Int nextNodePosition { get; set; } = new Vector2Int(0, 0);


        // 位置更新方法，允许重写
        public virtual void UpdatePosition()
        {
            // 更新位置
            position = Pathfinding.UpdateMovement(this);
        }

        #endregion

        #region 生产属性
        // 生产所需时间，单位秒，默认0
        public float ProductionTime { get; set; } = 0.0f;
        // 生产所需资源（战争资金）默认0
        public int ProductionCost { get; set; } = 0;
        // 生产前提条件列表，需要满足此条件才会出现在生产列表里，默认空字符串
        public string[] Prerequisite { get; set; } = Array.Empty<string>();
        // 提供生产条件，当该单位被生产时提供的生产前提单位，默认空字符串，此属性应被游戏初始化时设置
        public string[] Prerequisite_Provided { get; set; } = Array.Empty<string>();
        // 生产阵营白名单：只允许这些阵营生产此单位，默认空数组表示所有阵营均可生产
        public string[] Faction_Whitelist { get; set; } = Array.Empty<string>();
        // 生产阵营黑名单：不允许这些阵营生产此单位，默认空数组表示所有阵营均可生产
        public string[] Faction_Blacklist { get; set; } = Array.Empty<string>();
        // 组内生产数量限制，默认0表示不限制
        public int GroupLimit { get; set; } = 0;
        // 全局生产数量限制，默认0表示不限制
        public int GlobalLimit { get; set; } = 0;

        #endregion

        /// <summary>
        /// 游戏逻辑帧更新方法，允许重写
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void GameUpdate(TimeSpan deltaTime)
        {
            // 游戏对象逻辑帧更新逻辑

        }

        /// <summary>
        /// 在游戏内添加游戏对象，添加对象必须有无参数构造函数
        /// </summary>
        /// <param name="type">要添加的对象类</param>
        /// <exception cref="Exception"></exception>
        public static void AddToGame(Type type)
        {
            // 通过反射查找调用 GameUnit 类的静态方法 CreateInstance 来创建实例
            // CreateInstance 类接受一个 Type 类型的参数，不返回，它会将 type 保存到游戏引擎内
            var method = type.GetMethod("CreateInstance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (method != null)
            {
                method.Invoke(null, new object[] { type });
            }
            else
            {
                throw new Exception($"Type {type.FullName} does not have a static method CreateInstance.");
            }
        }
    }
}
