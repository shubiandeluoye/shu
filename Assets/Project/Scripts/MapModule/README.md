# MapModule 地图模块

采用纯C#核心层与Unity表现层分离的设计，实现高内聚低耦合的地图系统。

## 目录结构


MapModule/
├── Core/ # 核心层（纯C#）
│ ├── Data/ # 数据定义
│ │ ├── MapConfig.cs # 地图配置
│ │ ├── ShapeConfig.cs # 形状配置
│ │ ├── MapEvents.cs # 事件定义
│ │ └── ShapeState.cs # 状态定义
│ ├── Shapes/ # 形状实现
│ │ ├── IShape.cs # 形状接口
│ │ ├── BaseShape.cs # 形状基类
│ │ ├── CircleShape.cs # 圆形实现
│ │ ├── RectangleShape.cs # 矩形实现
│ │ ├── TriangleShape.cs # 三角形实现
│ │ └── TrapezoidShape.cs # 梯形实现
│ ├── States/ # 状态机实现
│ │ ├── MapStateBase.cs # 状态基类
│ │ ├── MapStateMachine.cs # 状态机
│ │ ├── MapInitializingState.cs
│ │ ├── MapPlayingState.cs
│ │ └── MapPausedState.cs
│ ├── Systems/ # 系统实现
│ │ ├── MapManager.cs # 地图管理器
│ │ ├── MapEventSystem.cs # 事件系统
│ │ ├── CentralAreaSystem.cs # 中心区域系统
│ │ ├── ShapeSystem.cs # 形状系统
│ │ └── ShapeFactory.cs # 形状工厂
│ └── Utils/ # 工具类
│ ├── MathUtils.cs # 数学工具
│ ├── TimeUtils.cs # 时间工具
│ ├── ShapeUtils.cs # 形状工具
│ └── MapDebugger.cs # 调试工具
└── Unity/ # Unity实现层
├── Components/ # Unity组件
│ ├── MapController.cs # 地图控制器
│ └── ShapeController.cs # 形状控制器
├── Configs/ # Unity配置
│ ├── MapConfigSO.cs # 地图配置SO
│ ├── ShapeTypeSO.cs # 形状类型SO
│ └── ShapeConfigSO.cs # 形状配置SO
├── Views/ # 视图实现
│ ├── IShapeView.cs # 视图接口
│ ├── BaseShapeView.cs # 视图基类
│ ├── CircleShapeView.cs # 圆形视图
│ ├── RectangleShapeView.cs # 矩形视图
│ ├── TriangleShapeView.cs # 三角形视图
│ └── TrapezoidShapeView.cs # 梯形视图
└── Utils/ # Unity工具
└── MapResourceManager.cs # 资源管理器

# MapModule 地图模块

采用纯C#核心层与Unity表现层分离的设计，实现高内聚低耦合的地图系统。



## 核心功能

### 1. 形状系统
- Circle: 收集子弹（21个）后消失
- Rectangle: 5*8网格，击中格子消失
- Triangle: 左右侧被击中产生旋转
- Trapezoid: 上下底子弹传送，下底加速

### 2. 状态管理
- Initializing: 初始化状态
- Playing: 游戏进行状态
- Paused: 暂停状态

### 3. 事件系统
- 系统事件（初始化、状态改变等）
- 形状事件（击中、消失等）
- 区域事件（状态变化等）

## 特点

### 1. 纯C#核心实现
- 不依赖Unity
- 可独立测试
- 跨平台支持
- 高内聚低耦合

### 2. 数据驱动
- ScriptableObject配置
- 事件驱动通信
- 状态机管理
- 对象池复用

### 3. 扩展性设计
- 接口解耦
- 基类复用
- 组件化结构
- 易于扩展

### 4. 调试支持
- 日志系统
- 状态监控
- 可视化调试
- 性能优化

## 使用示例
csharp
// 1. 创建并初始化管理器
var manager = new MapManager();
var config = new MapConfig
{
CentralAreaSize = new Vector2D(3, 10),
ActiveAreaSize = new Vector2D(3, 7),
ShapeChangeInterval = 40f
};
manager.Initialize(config);
// 2. 订阅事件
MapEventSystem.Instance.Subscribe(MapEvents.ShapeChanged, OnShapeChanged);
MapEventSystem.Instance.Subscribe(MapEvents.ShapeHit, OnShapeHit);
// 3. 更新系统
void Update(float deltaTime)
{
manager.Update(deltaTime);
}
// 4. 状态切换
manager.ChangeState("Playing");

## 注意事项
1. Core层不要引用Unity相关代码
2. 通过事件系统进行通信
3. 使用TimeUtils管理时间
4. 使用MathUtils进行数学计算
5. 配置数据通过SO文件管理
6. 视觉表现在Unity层实现

