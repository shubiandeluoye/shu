
PlayerModule/
├── Core/                         # 核心层（纯C#）
│   ├── Data/                     # 数据定义
│   │   ├── PlayerConfig.cs       # 配置数据
│   │   ├── PlayerEvents.cs       # 事件定义
│   │   └── PlayerState.cs        # 状态定义
│   └── Systems/                  # 系统实现
│       ├── PlayerManager.cs      # 玩家管理器
│       ├── PlayerEventSystem.cs  # 事件系统
│       ├── HealthSystem.cs       # 生命系统
│       ├── MovementSystem.cs     # 移动系统
│       └── ShootingSystem.cs     # 射击系统
└── Unity/                        # Unity实现层
    ├── Components/               # Unity组件
    │   ├── PlayerController.cs   # 玩家控制器
    │   ├── PlayerAnimator.cs     # 动画控制器
    │   └── PlayerInput.cs        # 输入控制器
    └── Systems/                  # Unity系统实现
        ├── UnityMovementSystem.cs  # 移动实现
        └── UnityShootingSystem.cs  # 射击实现

        
## 核心层（Core）

### Data
1. PlayerConfig.cs
- 负责：配置数据定义
- 包含：移动配置、生命值配置、射击配置
- 特点：纯数据结构，无业务逻辑

2. PlayerEvents.cs
- 负责：事件定义
- 包含：事件数据结构、事件名称常量
- 特点：统一事件管理

3. PlayerState.cs
- 负责：状态定义和接口声明
- 包含：玩家状态、系统接口
- 特点：定义核心数据结构

### Systems
1. PlayerManager.cs
- 负责：玩家生命周期管理
- 功能：玩家创建、销毁、状态管理
- 特点：单例模式

2. PlayerEventSystem.cs
- 负责：事件分发
- 功能：事件订阅、发布、处理
- 特点：观察者模式

3. HealthSystem.cs
- 负责：生命值管理
- 功能：伤害处理、治疗、无敌时间
- 特点：纯逻辑实现

4. MovementSystem.cs
- 负责：位置更新
- 功能：移动计算、碰撞检测、击退
- 特点：不依赖物理引擎

5. ShootingSystem.cs
- 负责：射击逻辑
- 功能：射击控制、弹道计算
- 特点：纯数学计算

## Unity实现层（Unity）

### Components
1. PlayerController.cs
- 负责：组件管理和初始化
- 功能：系统创建、事件路由
- 特点：桥接Core和Unity

2. PlayerAnimator.cs
- 负责：动画控制
- 功能：动画状态管理、特效控制
- 特点：处理表现层

3. PlayerInput.cs
- 负责：输入处理
- 功能：输入映射、动作转换
- 特点：适配输入系统

### Systems
1. UnityMovementSystem.cs
- 负责：物理移动实现
- 功能：位置同步、物理碰撞
- 特点：Unity物理系统集成

2. UnityShootingSystem.cs
- 负责：射击效果实现
- 功能：特效播放、音效处理
- 特点：Unity表现层实现

## 使用示例
csharp
// 创建玩家配置
var config = new PlayerConfig
{
MovementConfig = new MovementConfig { MoveSpeed = 5f },
HealthConfig = new HealthConfig { MaxHealth = 100 }
};
// 注册玩家
PlayerManager.Instance.RegisterPlayer(1, config);
// 订阅事件
PlayerEventSystem.Instance.Subscribe(PlayerEvents.PlayerCreated, OnPlayerCreated);
// Unity层实现
public class PlayerController : MonoBehaviour, IPlayerManager
{
private void Awake()
{
var state = PlayerManager.Instance.GetPlayerState(playerId);
unityMovement.Initialize(movementSystem);
}
}

## 特点
1. 分层设计：核心逻辑与实现分离
2. 事件驱动：系统间通过事件通信
3. 配置驱动：数值配置集中管理
4. 模块化设计：系统职责明确

## 注意事项
1. Core层不要引用Unity相关代码
2. 通过接口和事件进行跨层通信
3. 配置数据应该是纯数据结构
4. Unity层负责具体的表现效果

主要改进：
统一了目录结构的展示方式
更清晰地区分了Core层和Unity层
为每个组件添加了更详细的说明
简化了使用示例
移除了重复的内容
