# Core 模块说明文档

## 相关代码
- [EventManager.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Core/EventSystem/EventManager.cs)
- [GameEvents.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Core/EventSystem/GameEvents.cs)
- [StateMachine.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Core/FSM/StateMachine.cs)
- [ObjectPool.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Core/ObjectPool/ObjectPool.cs)
- [Singleton.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Core/Singleton/Singleton.cs)
- [TimerManager.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/Core/TimerManager.cs)

## 模块结构
```
Core/
├── EventSystem/        # 事件系统
│   ├── EventManager.cs
│   └── GameEvents.cs
├── FSM/               # 状态机系统
│   ├── GameState.cs
│   ├── IState.cs
│   └── StateMachine.cs
├── Network/           # 网络接口
│   └── INetworkAuthority.cs
├── ObjectPool/        # 对象池
│   ├── IPoolable.cs
│   └── ObjectPool.cs
├── Singleton/         # 单例模式
│   └── Singleton.cs
└── TimerManager.cs    # 计时器管理
```

## 1. 事件系统 (EventSystem)
### 使用方法
```csharp
// 注册事件
EventManager.Instance.AddListener<PlayerDamageEvent>(OnPlayerDamage);

// 触发事件
EventManager.Instance.TriggerEvent(new PlayerDamageEvent { Damage = 10 });

// 移除事件
EventManager.Instance.RemoveListener<PlayerDamageEvent>(OnPlayerDamage);
```

### 注意事项
- ⚠️ 必须在OnDisable/OnDestroy中移除事件监听
- ⚠️ 避免在事件回调中触发同类型事件（防止循环）
- ✅ 事件数据结构应该是struct而不是class

## 2. 状态机系统 (FSM)
### 使用方法
```csharp
// 创建状态机
var stateMachine = new StateMachine();

// 添加状态
stateMachine.AddState(new PlayingState());
stateMachine.AddState(new PausedState());

// 切换状态
stateMachine.ChangeState<PlayingState>();
```

### 注意事项
- ⚠️ 状态切换时会自动调用Exit和Enter
- ⚠️ 避免在状态切换时再次切换状态
- ✅ 状态类应该实现IState接口

## 3. 对象池系统 (ObjectPool)
### 使用方法
```csharp
// 创建对象池
var bulletPool = new ObjectPool<BulletController>(
    createFunc: () => Instantiate(bulletPrefab),
    actionOnGet: (bullet) => bullet.gameObject.SetActive(true),
    actionOnRelease: (bullet) => bullet.gameObject.SetActive(false)
);

// 获取对象
var bullet = bulletPool.Get();

// 释放对象
bulletPool.Release(bullet);
```

### 注意事项
- ⚠️ 使用对象前必须重置状态
- ⚠️ 避免在对象激活/关闭时进行复杂操作
- ✅ 对象最好实现IPoolable接口

## 4. 单例模式 (Singleton)
### 使用方法
```csharp
public class GameManager : Singleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        // 自定义初始化
    }
}
```

### 注意事项
- ⚠️ 避免在Awake中访问其他单例
- ⚠️ 单例对象应该持续整个游戏生命周期
- ✅ 使用protected override void Awake()

## 5. 计时器管理 (TimerManager)
### 使用方法
```csharp
// 创建计时器
TimerManager.Instance.CreateTimer(
    duration: 3f,
    onComplete: () => Debug.Log("Timer completed!"),
    isLooping: false
);

// 暂停/恢复所有计时器
TimerManager.Instance.PauseAll();
TimerManager.Instance.ResumeAll();
```

### 注意事项
- ⚠️ 计时器回调中避免耗时操作
- ⚠️ 场景切换时记得清理计时器
- ✅ 优先使用对象池而不是直接创建

## 性能优化建议
1. 事件系统
   - 大量事件时考虑使用对象池缓存事件数据
   - 频繁事件考虑批处理

2. 对象池
   - 预热对象池（提前创建对象）
   - 设置合理的最大容量

3. 计时器
   - 避免创建过多短时计时器
   - 考虑使用协程替代短时计时器

## 禁止修改项
1. 核心接口定义
2. 单例模式实现
3. 事件系统的基础架构
4. 对象池的内存管理
5. 状态机的状态切换逻辑

## 可以修改项
1. 事件数据结构
2. 具体状态类实现
3. 对象池的预热策略
4. 计时器的回调逻辑
5. 具体单例类的业务逻辑 