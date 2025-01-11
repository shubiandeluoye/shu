


# 第一阶段开发文档

## 前置信息
### 1. 仓库信息
- 仓库地址：https://github.com/shubiandeluoye/MyProject04_New
- 基础分支：main


### 2. 环境配置
- Unity版本：2022.3 LTS
- 必要插件：
  * URP 14.0.10
  * DOTween Pro
  * Input System
  * Fusion

### 3. 分支策略
- main：主分支（稳定版本）
- develop：开发主分支
- develop_phase1：第一阶段开发分支
- feature/*：具体功能分支

### 4. 提交流程
1. 从develop_phase1创建feature分支
2. 在feature分支开发
3. 提交到feature分支
4. 合并回develop_phase1

# 第一阶段开发文档

## 一、项目架构（15 ACU）

### 1. 文件结构规范
Assets/
├── Project
│ ├── Scripts/
│ │ ├── Core/ // 核心系统
│ │ │ ├── Singleton/ // 单例基类
│ │ │ ├── FSM/ // 状态机
│ │ │ ├── ObjectPool/ // 对象池
│ │ │ └── EventSystem/ // 事件系统
│ │ ├── Managers/ // 管理器
│ │ │ ├── GameManager
│ │ │ ├── InputManager
│ │ │ └── AudioManager
│ │ ├── Player/ // 玩家相关
│ │ ├── Bullets/ // 子弹系统
│ │ ├── CenterShape/ // 中心形状
│ │ └── UI/ // UI系统
│ ├── Prefabs/
│ ├── Art/
│ └── Scenes/


### 2. 标签系统
csharp
public static class GameTags
{
public const string Player = "Player";
public const string Bullet = "Bullet";
public const string Wall = "Wall";
public const string BounceArea = "BounceArea";
public const string DeadZone = "DeadZone";
public const string CenterShape = "CenterShape";
}


### 3. 核心系统实现要求

#### 3.1 单例基类

csharp
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
public static T Instance { get; private set; }
// 需实现：
// 1. 线程安全
// 2. 场景切换保持
// 3. 防止重复创建
}


#### 3.2 状态机系统

csharp
public interface IState
{
void Enter();
void Update();
void Exit();
}
public class StateMachine
{
// 需实现：
// 1. 状态切换
// 2. 状态更新
// 3. 状态历史
}


#### 3.3 对象池系统

csharp
public class ObjectPool : Singleton<ObjectPool>
{
// 需实现：
// 1. 预加载功能
// 2. 自动扩容
// 3. 对象回收
// 4. 池化管理
}


#### 3.4 事件系统

csharp
public class EventManager : Singleton<EventManager>
{
// 需实现：
// 1. 事件注册
// 2. 事件触发
// 3. 事件移除
// 4. 参数传递
}


### 4. 管理器实现要求

#### 4.1 游戏管理器

csharp
public class GameManager : Singleton<GameManager>
{
// 需实现：
// 1. 游戏状态管理
// 2. 场景管理
// 3. 游戏流程控制
}


#### 4.2 输入管理器

csharp
public class InputManager : Singleton<InputManager>
{
// 需实现：
// 1. 输入事件分发
// 2. 输入状态维护
// 3. 按键配置管理
}


## 二、开发规范

### 1. 代码规范
- 使用 PascalCase 命名类和方法
- 使用 camelCase 命名变量和参数
- 添加必要的注释
- 遵循单一职责原则

### 2. 性能要求
- 确保单例创建高效
- 对象池预加载合理
- 事件系统响应及时
- 内存占用最小化

### 3. 测试要求
- 单例创建销毁测试
- 状态切换测试
- 对象池压力测试
- 事件系统并发测试

## 三、提交要求

### 1. 代码提交
- 清晰的提交信息
- 合理的提交粒度
- 完整的功能测试
- 符合代码规范

### 2. 文档要求
- 系统设计文档
- 接口说明文档
- 测试报告
- 已知问题列表

## 四、优先级说明

1. 第一优先级（必须实现）：
- 单例基类
- 基础状态机
- 简单对象池
- 核心事件系统

2. 第二优先级（条件允许实现）：
- 管理器框架
- 配置系统
- 日志系统

3. 预留优化项：
- 性能优化
- 内存优化
- 扩展功能