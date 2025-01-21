# Map Module 文档

## 1. 模块结构

### 1.1 核心类
- `MapSystemManager.cs`
  - 位置：Assets/Project/Scripts/MapModule/Systems/
  - 功能：地图系统总管理器，负责初始化和管理所有子系统
  - 依赖：CentralAreaSystem, ShapeSystem

- `CentralAreaSystem.cs`
  - 位置：Assets/Project/Scripts/MapModule/Systems/
  - 功能：中央区域管理，处理形状的生成区域限制
  - 依赖：MapConfig

- `ShapeSystem.cs`
  - 位置：Assets/Project/Scripts/MapModule/Systems/
  - 功能：形状管理系统，处理形状的生命周期
  - 依赖：ShapeFactory

### 1.2 形状类
- `BaseShape.cs`
  - 位置：Assets/Project/Scripts/MapModule/Shapes/Base/
  - 功能：所有形状的基类，实现基础网络同步和通用功能
  - 依赖：IShape, NetworkBehaviour

具体形状实现：
- `CircleShape.cs`
  - 位置：Assets/Project/Scripts/MapModule/Shapes/Types/
  - 功能：收集21个子弹后消失
  - 特有属性：bulletCount

- `RectShape.cs`
  - 位置：Assets/Project/Scripts/MapModule/Shapes/Types/
  - 功能：5*8网格系统，击中格子消失，30秒后消失
  - 特有属性：gridState, remainingTime

- `TriangleShape.cs`
  - 位置：Assets/Project/Scripts/MapModule/Shapes/Types/
  - 功能：左右侧被击中产生不同方向旋转
  - 特有属性：currentRotation, rotationDirection

- `TrapezoidShape.cs`
  - 位置：Assets/Project/Scripts/MapModule/Shapes/Types/
  - 功能：上下底子弹传送，下底进入会加速，30秒后消失
  - 特有属性：lastBulletTime, remainingTime

### 1.3 数据类
- `ShapeConfig.cs`
  - 位置：Assets/Project/Scripts/MapModule/Shapes/Data/
  - 功能：形状运行时配置数据
  - 关键字段：
    - Type: 形状类型
    - Size: 大小
    - Duration: 持续时间
    - 各形状特有配置

- `ShapeConfigSO.cs`
  - 位置：Assets/Project/Scripts/MapModule/Shapes/Data/
  - 功能：形状ScriptableObject配置
  - 配置项：
    - 基础配置：类型、大小、持续时间
    - 圆形配置：子弹容量
    - 矩形配置：网格大小
    - 三角形配置：旋转速度
    - 梯形配置：上下底宽度、子弹延迟

- `ShapeEvents.cs`
  - 位置：Assets/Project/Scripts/MapModule/Shapes/Data/
  - 功能：定义所有形状相关事件
  - 事件类型：
    - ShapeHitEvent: 形状被击中
    - BulletCollectedEvent: 子弹收集
    - ShapeActionEvent: 形状行为

- `ShapeState.cs`
  - 位置：Assets/Project/Scripts/MapModule/Shapes/Data/
  - 功能：形状状态数据
  - 状态数据：
    - 基础状态：类型、位置、是否激活
    - 各形状特有状态

### 1.4 工具类
- `ShapeUtils.cs`
  - 位置：Assets/Project/Scripts/MapModule/Utils/
  - 功能：形状相关的数学计算工具

- `MapDebugger.cs`
  - 位置：Assets/Project/Scripts/MapModule/Utils/
  - 功能：调试工具，用于可视化和日志

### 1.5 接口
- `IShape.cs`
  - 位置：Assets/Project/Scripts/MapModule/Interfaces/
  - 功能：定义形状基本接口
  - 主要方法：
    - Initialize
    - HandleBulletHit
    - GetState
    - SetPosition
    - Reset

## 2. 网络同步说明
所有需要同步的状态都使用 [Networked] 属性标记：
- CircleShape: bulletCount
- RectShape: gridState, remainingTime
- TriangleShape: currentRotation, rotationDirection
- TrapezoidShape: lastBulletTime, currentRotation, rotationDirection, remainingTime

## 3. 调试功能
在 MapSystemManager 中设置 enableDebug 为 true 可开启：
- 形状碰撞边界可视化
- 中央区域显示
- 活动区域显示
- 网格状态可视化
- 详细日志输出

## 4. 注意事项
1. 所有形状更新都在 FixedUpdateNetwork 中进行
2. 状态变更需要检查 StateAuthority
3. 配置值都有验证确保在合理范围内
4. 事件系统用于处理形状行为通知 