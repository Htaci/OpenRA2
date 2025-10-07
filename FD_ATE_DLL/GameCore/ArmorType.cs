public class ArmorType
{
    // 唯一标识符，护甲的名字
    public string Name { get; }

    // 说明：防御值和防御倍率都是只有在被无指定护甲的武器攻击时才有效

    // 防御倍率，1.0表示无变化，<1.0表示减少伤害，>1.0表示增加伤害，当被无指定护甲的武器攻击时有效
    public float DefenseMultiplier { get; }

    // 防御值，当被无指定护甲的武器攻击时有效
    public float DefenseValue { get; }

    // 护甲是否无敌
    public bool IsInvulnerable { get; set; } = false;

    // 构造函数
    public ArmorType(string name, float defenseMultiplier = 1.0f, float defenseValue = 0.0f)
    {
        Name = name;
        DefenseMultiplier = defenseMultiplier;
    }

    // 注册表
    private static readonly Dictionary<string, ArmorType> registry = new();

    // 注册新护甲类型
    public static void Register(ArmorType armorType)
    {
        // 验证护甲是否已存在，如果不存在则添加
        if (!registry.ContainsKey(armorType.Name))
        {
            registry[armorType.Name] = armorType;
        }
    }

    // 获取护甲类型
    public static ArmorType? Get(string name)
    {
        registry.TryGetValue(name, out var armorType);
        return armorType;
    }
}