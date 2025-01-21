# Combat Module 文档

## 1. 模块结构

### 1.1 核心类
- `CombatSystemManager.cs`
  - 位置：Assets/Project/Scripts/CombatModule/
  - 功能：战斗系统总管理器，模块唯一对外接口
  - 依赖：DamageCalculator, StatusEffectSystem

- `DamageCalculator.cs`
  - 位置：Assets/Project/Scripts/CombatModule/Core/
  - 功能：伤害计算器，处理所有伤害计算逻辑
  - 依赖：DamageConfig

- `StatusEffectSystem.cs`
  - 位置：Assets/Project/Scripts/CombatModule/Core/
  - 功能：状态效果系统，管理所有状态效果
  - 依赖：IStatusEffectHandler

### 1.2 数据类
- `CombatConfig.cs`
  - 位置：Assets/Project/Scripts/CombatModule/Data/
  - 功能：战斗配置数据
  - 关键配置：
    - DamageConfig: 伤害相关配置
    - StatusEffectConfig: 状态效果相关配置

- `DamageData.cs`
  - 位置：Assets/Project/Scripts/CombatModule/Data/
  - 功能：伤害数据结构
  - 关键数据：
    - BaseDamage: 基础伤害
    - DamageType: 伤害类型
    - CritChance: 暴击几率

- `CombatEvents.cs`
  - 位置：Assets/Project/Scripts/CombatModule/Data/
  - 功能：战斗相关事件定义
  - 事件类型：
    - BulletHitEvent: 子弹命中
    - SkillHitEvent: 技能命中
    - StatusEffectEvent: 状态效果事件

## 2. 回调说明
所有伤害处理通过回调机制实现：
csharp
// 注册伤害回调
CombatSystemManager.Instance.RegisterDamageCallback(gameObject, OnDamageReceived);
// 回调方法
private void OnDamageReceived(int damage)
{
// 处理伤害
}

## 3. 主要功能
1. 伤害处理
   - 基础伤害计算
   - 暴击判定
   - 伤害类型修正
   - 伤害回调通知

2. 状态效果
   - 效果添加/移除
   - 效果叠加
   - 持续伤害
   - 状态更新

3. 战斗事件
   - 子弹命中
   - 技能命中
   - 状态效果变化

## 4. 注意事项
1. 所有方法都在主线程调用
2. 注册的回调需要在对象销毁时取消注册
3. 状态效果会自动更新和清理
4. 配置值需要在 Inspector 中设置