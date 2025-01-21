# Player Module 文档

## 1. 模块结构

### 1.1 核心类
- `PlayerSystemManager.cs`
  - 位置：Assets/Project/Scripts/PlayerModule/
  - 功能：玩家系统总管理器，负责初始化和管理所有子系统
  - 依赖：HealthSystem, MovementSystem, ShootingSystem

- `HealthSystem.cs`
  - 位置：Assets/Project/Scripts/PlayerModule/Systems/
  - 功能：生命值管理，处理伤害和治疗
  - 依赖：HealthConfig

- `MovementSystem.cs`
  - 位置：Assets/Project/Scripts/PlayerModule/Systems/
  - 功能：移动系统，处理位置和击退效果
  - 依赖：MovementConfig

- `ShootingSystem.cs`
  - 位置：Assets/Project/Scripts/PlayerModule/Systems/
  - 功能：射击系统，处理子弹发射和角度
  - 依赖：ShootingConfig

### 1.2 数据类
- `PlayerConfig.cs`
  - 位置：Assets/Project/Scripts/PlayerModule/Data/
  - 功能：玩家配置数据
  - 关键配置：
    - MovementConfig: 移动相关配置
    - HealthConfig: 生命值相关配置
    - ShootingConfig: 射击相关配置

- `PlayerEvents.cs`
  - 位置：Assets/Project/Scripts/PlayerModule/Data/
  - 功能：定义所有玩家相关事件
  - 事件类型：
    - PlayerDamageEvent: 玩家受伤
    - PlayerStunEvent: 玩家眩晕
    - PlayerHealthChangedEvent: 生命值变化
    - PlayerDeathEvent: 玩家死亡
    - PlayerHealEvent: 玩家治疗
    - PlayerOutOfBoundsEvent: 出界事件
    - PlayerInputData: 输入数据

- `PlayerState.cs`
  - 位置：Assets/Project/Scripts/PlayerModule/Data/
  - 功能：玩家状态数据和枚举定义
  - 状态数据：
    - Health: 当前生命值
    - Position: 当前位置
    - ShootAngle: 射击角度
    - IsStunned: 是否眩晕
    - CurrentBulletType: 当前子弹类型
  - 枚举定义：
    - BulletType: 子弹类型
    - ShootInputType: 射击输入类型
    - ModifyHealthType: 生命值修改类型
    - DeathReason: 死亡原因

## 2. 网络同步说明
所有需要同步的状态都使用 [Networked] 属性标记：
- IsInitialized: 是否初始化
- CurrentHealth: 当前生命值
- CurrentPosition: 当前位置
- CurrentAngle: 当前角度

## 3. 主要功能
1. 生命值系统
   - 伤害处理
   - 治疗处理
   - 无敌时间
   - 死亡检测

2. 移动系统
   - 8方向移动
   - 击退效果
   - 眩晕处理
   - 边界检查

3. 射击系统
   - 多角度射击
   - 子弹类型切换
   - 射击冷却
   - 子弹生成

## 4. 注意事项
1. 所有状态更新需要检查 StateAuthority
2. 输入处理需要确保玩家已初始化
3. 配置值需要在合理范围内
4. 事件系统用于处理玩家状态变化通知
5. 移动和射击需要考虑网络延迟 