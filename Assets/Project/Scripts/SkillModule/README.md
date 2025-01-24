# SkillModule 技能模块

纯C#实现的技能系统，不依赖任何游戏引擎。

## Core 文件夹
核心功能文件：

1. BaseSkill.cs
- 技能基类
- 负责：基础技能属性、冷却管理、技能状态控制、事件分发
- 主要功能：Execute、CanUse、Use等

2. PassiveSkillBase.cs
- 被动技能基类
- 负责：技能基础属性、触发条件判断、冷却管理
- 主要功能：Trigger、CanTrigger、ValidateParameters等

3. SkillConfig.cs
- 技能配置基类
- 负责：基础技能属性配置、技能类型定义
- 主要功能：IsValid等

4. SkillContext.cs
- 技能执行上下文
- 负责：存储技能执行时的所有相关信息
- 主要功能：链式调用设置属性

5. SkillManager.cs
- 技能管理器
- 负责：技能配置管理、技能实例管理
- 主要功能：CreateSkill、UseSkill、RegisterSkill等

## Events 文件夹
事件系统文件：

1. EffectEvents.cs
- 效果相关事件定义
- AddEffectEvent：添加效果事件
- RemoveEffectEvent：移除效果事件
- EffectStateChangeEvent：效果状态变更事件

2. SkillEvents.cs
- 技能相关事件定义
- SkillStartEvent：技能开始事件
- SkillEndEvent：技能结束事件
- SkillCooldownEvent：技能冷却事件
- SkillStateChangeEvent：技能状态变更事件
- EventNames：预定义事件名称

## Types 文件夹
类型定义文件：

1. SkillDataTypes.cs
- 技能数据类型定义
- SkillState：技能状态枚举
- SkillData：基础技能数据
- SkillEventData：技能事件数据

2. SkillTypes.cs
- 基础类型枚举定义
- SkillType：技能类型
- EffectType：效果类型
- TargetType：目标类型

3. EffectData.cs
- 效果数据结构定义
- EffectData：基础效果数据
- ProjectileEffectData：投射物效果数据
- AreaEffectData：区域效果数据
- BarrierEffectData：屏障效果数据

## Utils 文件夹
工具类：

1. SkillUtils.cs
- 通用工具方法
- GetCurrentTime：获取当前时间
- CalculateDistance：计算距离
- NormalizeAngle：角度规范化
- IsInRange：范围检查
- GenerateUniqueId：生成唯一ID

## 示例技能实现

1. BoxSkill
- BoxSkill.cs：盒子技能实现
- BoxConfig.cs：盒子技能配置

2. BarrierSkill
- BarrierSkill.cs：屏障技能实现
- BarrierConfig.cs：屏障技能配置

3. HealSkill
- HealSkill.cs：治疗技能实现
- HealConfig.cs：治疗技能配置

4. ShootSkill
- ShootSkill.cs：射击技能实现
- ShootConfig.cs：射击技能配置

## 使用示例
csharp
// 创建技能配置
var config = new ShootConfig
{
SkillId = 1,
SkillName = "基础射击",
BulletSpeed = 10f,
BulletDamage = 20f
};
// 注册技能配置
SkillManager.Instance.RegisterSkill(config);
// 创建技能实例
SkillManager.Instance.CreateSkill(1, owner);
// 使用技能
var context = new SkillContext()
.WithPosition(new Vector3(0, 0, 0))
.WithDirection(new Vector3(1, 0, 0));
SkillManager.Instance.UseSkill(1, context);
// 订阅技能事件
SkillEventSystem.Instance.Subscribe(EventNames.SkillStart, OnSkillStart);

## 特点
1. 纯C#实现，不依赖特定游戏引擎
2. 基于事件的解耦设计
3. 完整的技能生命周期管理
4. 灵活的配置系统
5. 支持主动和被动技能
6. 易于扩展的模块化设计