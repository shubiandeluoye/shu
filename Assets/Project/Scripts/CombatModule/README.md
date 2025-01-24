# Combat 模块说明文档

## 相关代码

### 核心系统
- [CombatSystemManager.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/CombatModule/CombatSystemManager.cs)
  - 战斗系统总管理器
  - 初始化战斗系统
  - 管理战斗状态
  - 协调各子系统

### Core/ - 核心功能
- [DamageCalculator.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/CombatModule/Core/DamageCalculator.cs)
  - 伤害计算系统
  - 处理各类伤害公式
  - 计算防御减伤
  - 处理暴击机制

- [StatusEffectSystem.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/CombatModule/Core/StatusEffectSystem.cs)
  - 状态效果系统
  - 管理buff/debuff
  - 处理持续效果
  - 控制状态时间

## 配置详解

### 1. 伤害配置
```csharp
[System.Serializable]
public class DamageConfig
{
    public float BaseDamage = 10f;        // 基础伤害
    public float CriticalMultiplier = 2f;  // 暴击倍率
    public float CriticalChance = 0.2f;    // 暴击概率
    public DamageType DamageType;          // 伤害类型
    public bool CanBeBlocked = true;       // 是否可被格挡
    public float ArmorPenetration = 0f;    // 护甲穿透
}

public enum DamageType
{
    Physical,   // 物理伤害
    Magic,      // 魔法伤害
    True,       // 真实伤害
    Pure        // 纯粹伤害
}
```

### 2. 状态效果配置
```csharp
[System.Serializable]
public class StatusEffectConfig
{
    public StatusEffectType Type;          // 效果类型
    public float Duration = 5f;            // 持续时间
    public float TickInterval = 1f;        // 触发间隔
    public float Magnitude = 1f;           // 效果强度
    public bool CanStack = false;          // 是否可叠加
    public int MaxStacks = 1;              // 最大叠加层数
}

public enum StatusEffectType
{
    Burn,       // 燃烧
    Freeze,     // 冰冻
    Poison,     // 中毒
    Stun,       // 眩晕
    Slow,       // 减速
    Boost,      // 增益
    Shield      // 护盾
}
```

## 使用示例

### 1. 伤害计算
```csharp
public class DamageExample : MonoBehaviour
{
    [SerializeField] private DamageConfig damageConfig;
    private DamageCalculator damageCalculator;

    public float CalculateDamage(float targetArmor)
    {
        return damageCalculator.Calculate(
            damageConfig.BaseDamage,
            targetArmor,
            damageConfig.ArmorPenetration,
            out bool isCritical
        );
    }
}
```

### 2. 状态效果应用
```csharp
public class StatusEffectExample : MonoBehaviour
{
    [SerializeField] private StatusEffectConfig effectConfig;
    private StatusEffectSystem effectSystem;

    public void ApplyEffect(GameObject target)
    {
        effectSystem.ApplyEffect(
            target,
            effectConfig.Type,
            effectConfig.Duration,
            effectConfig.Magnitude
        );
    }
}
```

## 战斗系统流程

### 1. 伤害处理流程
1. 伤害源发起攻击
2. 计算基础伤害
3. 检查暴击
4. 计算护甲减伤
5. 应用最终伤害
6. 触发相关效果
7. 更新目标状态

### 2. 状态效果流程
1. 效果触发检查
2. 验证目标状态
3. 检查效果叠加
4. 应用效果数值
5. 开始效果计时
6. 定期触发效果
7. 效果结束处理

## 注意事项

### 1. 伤害系统
- 确保伤害计算平衡
- 注意数值溢出
- 处理无敌状态
- 考虑网络同步
- 优化计算性能

### 2. 状态系统
- 控制效果持续时间
- 管理效果优先级
- 处理效果冲突
- 优化效果更新
- 注意内存占用

## 扩展功能

### 1. 自定义伤害类型
```csharp
public class CustomDamageType : IDamageType
{
    public float CalculateModifier(float baseDamage)
    {
        // 自定义伤害计算逻辑
        return baseDamage * GetCustomModifier();
    }
}
```

### 2. 自定义状态效果
```csharp
public class CustomStatusEffect : BaseStatusEffect
{
    public override void OnApply()
    {
        // 自定义效果应用逻辑
    }

    public override void OnTick()
    {
        // 自定义效果周期触发逻辑
    }
}
```

## 性能优化

### 1. 计算优化
- 使用查找表
- 缓存计算结果
- 批量处理伤害
- 优化数值精度
- 减少GC分配

### 2. 更新优化
- 使用对象池
- 优化效果更新
- 实现脏标记系统
- 合并状态更新
- 优化触发检查

## 调试工具

### 1. 伤害调试
```csharp
public class DamageDebugger : MonoBehaviour
{
    [SerializeField] private bool showDamageNumbers = true;
    [SerializeField] private bool logDamageDetails = true;

    private void LogDamage(float damage, bool isCritical)
    {
        if (logDamageDetails)
        {
            Debug.Log($"Damage dealt: {damage} (Critical: {isCritical})");
        }
    }
}
```

### 2. 状态效果调试
```csharp
public class StatusEffectDebugger : MonoBehaviour
{
    [SerializeField] private bool showEffectIcons = true;
    [SerializeField] private bool logEffectChanges = true;

    private void LogEffectChange(StatusEffectType type, float duration)
    {
        if (logEffectChanges)
        {
            Debug.Log($"Effect applied: {type} (Duration: {duration}s)");
        }
    }
}
```

## 常见问题解决

### 1. 伤害问题
- 检查伤害配置
- 验证计算公式
- 确认数值范围
- 检查网络同步
- 验证触发条件

### 2. 状态效果问题
- 检查效果配置
- 验证更新逻辑
- 确认移除条件
- 检查效果优先级
- 验证效果叠加

## 维护建议

### 1. 日常维护
- 平衡数值设计
- 监控性能指标
- 更新调试工具
- 优化计算方法
- 完善错误处理

### 2. 版本更新
- 记录更新日志
- 备份配置数据
- 测试新功能
- 验证兼容性
- 更新文档说明

## 修改限制

### 禁止修改
1. 核心接口和基类
   - IDamageType 接口
   - IStatusEffect 接口
   - BaseStatusEffect 基类
   - 这些接口和基类定义了系统的基本架构，修改会影响整个战斗系统

2. 伤害计算核心逻辑
   - DamageCalculator.cs 中的核心计算方法
   - 伤害类型枚举定义
   - 这些涉及游戏平衡性的核心逻辑

3. 状态效果基础框架
   - StatusEffectSystem.cs 中的效果应用机制
   - 效果更新循环
   - 效果移除机制

4. 网络同步相关
   - 伤害同步协议
   - 状态效果同步机制
   - 这些修改可能导致网络不同步

### 可以修改
1. 配置参数
   - DamageConfig 中的数值配置
   - StatusEffectConfig 中的效果参数
   - 这些参数用于调整游戏平衡性

2. 自定义实现
   - 创建新的伤害类型
   - 实现自定义状态效果
   - 添加新的效果触发条件
   - 扩展调试功能

3. 视觉表现
   - 伤害数字显示
   - 效果特效
   - UI提示
   - 音效反馈

4. 优化相关
   - 对象池实现
   - 缓存策略
   - 更新频率
   - 这些不影响核心逻辑的性能优化

### 需要审核的修改
1. 伤害公式调整
   - 需要通过游戏设计团队审核
   - 需要进行数值平衡测试
   - 需要考虑对现有内容的影响

2. 新增状态效果类型
   - 需要评估与现有效果的互动
   - 需要确保网络同步正确
   - 需要进行性能评估

3. 核心流程变更
   - 需要完整的测试验证
   - 需要确保向后兼容
   - 需要评估对其他系统的影响