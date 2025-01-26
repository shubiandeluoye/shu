# Player 模块说明文档

## 最新更新
### 2025.01.26
1. 移动系统优化
   - 移除了角度相关代码
   - 统一使用ShootPoint控制射击方向
   - 简化了移动逻辑

2. 射击系统重构
   - 移除了角度控制系统
   - 射击点完全由技能系统控制
   - 支持骨骼模型发射点配置

3. 输入系统优化
   - 移除了未使用的死区检测
   - 简化了输入处理逻辑
   - 优化了设备端口监听

## 发射点解决方案
### 1. Transform标记方式
优点：
- 直观易用
- 便于调试
- 灵活配置

缺点：
- 需要手动放置标记物
- 可能增加场景对象数量

### 2. 动画事件方式
优点：
- 精确控制发射时机
- 与动画系统集成好

缺点：
- 依赖动画系统
- 配置相对复杂

### 3. 骨骼名称映射
优点：
- 不需要额外对象
- 直接使用骨骼系统

缺点：
- 依赖骨骼命名
- 可能需要偏移调整

### 4. Socket系统
优点：
- 统一管理所有挂点
- 支持运行时修改
- 便于扩展

缺点：
- 系统相对复杂
- 需要额外管理Socket

## 建议使用方案
根据项目需求选择：
1. 简单项目：使用Transform标记
2. 动画密集：使用动画事件
3. 通用性强：使用Socket系统
4. 性能优先：使用骨骼映射

## 相关代码
### 核心管理器
- [PlayerSystemManager.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/PlayerModule/PlayerSystemManager.cs) - 玩家系统总管理器

### 数据相关
- [PlayerConfig.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/PlayerModule/Data/PlayerConfig.cs) - 玩家配置数据
- [PlayerEvents.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/PlayerModule/Data/PlayerEvents.cs) - 玩家相关事件
- [PlayerState.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/PlayerModule/Data/PlayerState.cs) - 玩家状态数据

### 系统相关
- [MovementSystem.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/PlayerModule/Systems/MovementSystem.cs) - 移动系统
- [HealthSystem.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/PlayerModule/Systems/HealthSystem.cs) - 生命值系统
- [ShootingSystem.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/PlayerModule/Systems/ShootingSystem.cs) - 射击系统

## 配置详解

### 1. 玩家基础配置 (PlayerConfig)
```csharp
[CreateAssetMenu(fileName = "NewPlayerConfig", menuName = "Game/Player/New Player Config")]
public class PlayerConfig
{
    public MovementConfig MovementConfig;
    public HealthConfig HealthConfig;
    public ShootingConfig ShootingConfig;
}

[System.Serializable]
public class MovementConfig
{
    public float MoveSpeed = 5f;        // 移动速度
    public float KnockbackDrag = 3f;    // 击退阻力
    public float MinX = -3.5f;          // X轴最小值
    public float MaxX = 3.5f;           // X轴最大值
    public float MinZ = -3.5f;          // Z轴最小值
    public float MaxZ = 3.5f;           // Z轴最大值
}

[System.Serializable]
public class HealthConfig
{
    public int MaxHealth = 100;         // 最大生命值
    public float InvincibilityTime = 0.5f; // 无敌时间
}

[System.Serializable]
public class ShootingConfig
{
    public float[] ShootAngles = { 0f, 30f, -30f }; // 射击角度
    public float ShootCooldown = 0.2f;  // 射击冷却
    public float BulletSpawnOffset = 0.5f; // 子弹生成偏移
}
```

### 2. 玩家状态 (PlayerState)
```csharp
public class PlayerState
{
    public int Health { get; set; }
    public Vector3 Position { get; set; }
    public float ShootAngle { get; set; }
    public bool IsStunned { get; set; }
    public BulletType CurrentBulletType { get; set; }
}
```

### 3. 玩家事件 (PlayerEvents)
```csharp
public struct PlayerDamageEvent
{
    public int Damage;
    public Vector3 DamageDirection;
    public bool HasKnockback;
}

public struct PlayerHealEvent
{
    public int HealAmount;
}

public struct PlayerDeathEvent
{
    public Vector3 DeathPosition;
    public int KillerId;
}

public struct PlayerInputData
{
    public bool HasMovementInput;
    public Vector3 MovementDirection;
    public bool HasShootInput;
    public ShootInputType ShootInputType;
    public bool IsAngleTogglePressed;
}
```

## 系统实现详解

### 1. 移动系统 (MovementSystem)
```csharp
public class MovementSystem
{
    private readonly MovementConfig config;
    private Vector3 currentPosition;
    private Vector3 currentVelocity;
    private Rigidbody rb;

    public void Initialize(Vector3 startPosition)
    {
        currentPosition = startPosition;
        currentVelocity = Vector3.zero;
    }

    public void HandleMovement(Vector3 direction)
    {
        if (isStunned) return;

        // 确保在XZ平面上移动
        Vector3 flatDirection = new Vector3(direction.x, 0, direction.z).normalized;
        currentVelocity = flatDirection * config.MoveSpeed;
        
        // 使用Rigidbody移动
        if (rb != null)
        {
            rb.velocity = new Vector3(currentVelocity.x, rb.velocity.y, currentVelocity.z);
        }
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (!isStunned)
        {
            Vector3 knockbackDir = new Vector3(direction.x, 0, direction.z).normalized;
            currentVelocity += knockbackDir * force;
        }
    }
}
```

### 2. 生命值系统 (HealthSystem)
```csharp
public class HealthSystem
{
    private readonly HealthConfig config;
    private int currentHealth;
    private float invincibilityEndTime;

    public void Initialize()
    {
        currentHealth = config.MaxHealth;
        invincibilityEndTime = 0f;
    }

    public void TakeDamage(int damage)
    {
        if (Time.time < invincibilityEndTime) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        invincibilityEndTime = Time.time + config.InvincibilityTime;

        EventManager.Instance.TriggerEvent(new PlayerDamageEvent 
        { 
            Damage = damage 
        });

        if (currentHealth <= 0)
        {
            EventManager.Instance.TriggerEvent(new PlayerDeathEvent());
        }
    }

    public void Heal(int amount)
    {
        int oldHealth = currentHealth;
        currentHealth = Mathf.Min(config.MaxHealth, currentHealth + amount);

        if (currentHealth > oldHealth)
        {
            EventManager.Instance.TriggerEvent(new PlayerHealEvent 
            { 
                HealAmount = currentHealth - oldHealth 
            });
        }
    }
}
```

### 3. 射击系统 (ShootingSystem)
```csharp
public class ShootingSystem
{
    private readonly ShootingConfig config;
    private float nextShootTime;
    private int currentAngleIndex;

    public void Initialize()
    {
        nextShootTime = 0f;
        currentAngleIndex = 0;
    }

    public void Shoot(Vector3 position, Vector3 direction)
    {
        if (Time.time < nextShootTime) return;

        float currentAngle = config.ShootAngles[currentAngleIndex];
        Vector3 rotatedDirection = Quaternion.Euler(0, currentAngle, 0) * direction;
        
        // 生成子弹
        Vector3 spawnPosition = position + rotatedDirection * config.BulletSpawnOffset;
        var bullet = BulletPool.Instance.Get();
        bullet.transform.position = spawnPosition;
        bullet.transform.rotation = Quaternion.LookRotation(rotatedDirection);
        
        nextShootTime = Time.time + config.ShootCooldown;
    }

    public void ToggleShootAngle()
    {
        currentAngleIndex = (currentAngleIndex + 1) % config.ShootAngles.Length;
    }
}
```

## 使用示例

### 1. 配置玩家
```csharp
// 在Unity编辑器中
[SerializeField] private PlayerConfig playerConfig;

void Awake()
{
    // 初始化各个系统
    movementSystem = new MovementSystem(playerConfig.MovementConfig);
    healthSystem = new HealthSystem(playerConfig.HealthConfig);
    shootingSystem = new ShootingSystem(playerConfig.ShootingConfig);

    // 设置初始状态
    movementSystem.Initialize(transform.position);
    healthSystem.Initialize();
    shootingSystem.Initialize();
}
```

### 2. 处理输入
```csharp
private void OnInputAction(InputAction.CallbackContext context)
{
    if (!IsInitialized) return;

    // 处理移动输入
    if (context.action.name == "Move")
    {
        Vector2 input = context.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        HandleMovementInput(new PlayerInputData 
        { 
            HasMovementInput = true,
            MovementDirection = moveDirection 
        });
    }
    
    // 处理射击输入
    if (context.action.name == "Shoot" && context.performed)
    {
        HandleShootInput(new PlayerInputData
        {
            HasShootInput = true,
            ShootInputType = ShootInputType.Normal
        });
    }
}
```

### 3. 网络同步
```csharp
[ServerRpc]
private void ServerMove(Vector3 direction)
{
    // 服务器端验证
    if (!IsValidMovement(direction)) return;

    // 更新位置
    movementSystem.HandleMovement(direction);
    
    // 同步到所有客户端
    ClientRpcMove(transform.position);
}

[ClientRpc]
private void ClientRpcMove(Vector3 newPosition)
{
    // 客户端更新位置
    if (!IsOwner)
    {
        transform.position = newPosition;
    }
}
```

## Unity组件要求

### 1. 必需组件
```csharp
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(NetworkObject))]
public class PlayerSystemManager : NetworkBehaviour
{
    private Rigidbody rb;
    private PlayerInput playerInput;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        
        // 设置Rigidbody属性
        rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                        RigidbodyConstraints.FreezeRotationZ;
    }
}
```

### 2. 可选组件
- AudioSource (音效)
- ParticleSystem (特效)
- Animator (动画)

## 禁止修改项
1. 核心系统架构
   - PlayerSystemManager的基础结构
   - 系统初始化流程
   - 网络同步基础逻辑

2. 事件系统
   - 事件定义
   - 事件触发机制
   - 事件处理流程

3. 输入系统
   - Input System的基础配置
   - 输入事件的处理流程
   - 输入数据结构

4. 物理系统
   - Rigidbody的基础设置
   - 碰撞检测机制
   - 物理更新频率

## 可以修改项
1. 玩家配置
   - 移动参数
   - 生命值设置
   - 射击配置
   - 边界范围

2. 表现效果
   - 动画系统
   - 特效系统
   - 音效系统
   - UI反馈

3. 游戏玩法
   - 技能组合
   - 伤害计算
   - 击退效果
   - 特殊状态

4. 扩展功能
   - 新的移动方式
   - 额外的状态效果
   - 自定义事件
   - 特殊技能

## 常见问题解决

### 1. 移动问题
- 症状：玩家无法移动
- 解决方案：
  1. 检查 Rigidbody 设置
  2. 确认 Input System 配置
  3. 验证移动边界设置
  4. 检查是否被击晕

### 2. 伤害计算
- 症状：伤害不正确
- 解决方案：
  1. 检查无敌时间
  2. 确认伤害计算公式
  3. 验证网络权限
  4. 检查碰撞器设置

### 3. 射击问题
- 症状：无法射击
- 解决方案：
  1. 检查冷却时间
  2. 确认子弹预制体
  3. 验证射击角度
  4. 检查对象池设置

### 4. 网络同步
- 症状：位置不同步
- 解决方案：
  1. 检查 NetworkObject
  2. 确认 RPC 调用
  3. 验证权限设置
  4. 检查网络状态

## 性能优化建议

### 1. 物理系统
- 使用 FixedUpdate 处理物理
- 适当设置 Rigidbody 约束
- 优化碰撞器形状
- 使用物理材质

### 2. 输入处理
- 使用 Input System 的事件模式
- 批量处理输入
- 减少每帧检查
- 使用输入缓冲

### 3. 特效系统
- 使用对象池
- 优化粒子系统
- 使用GPU Instancing
- 控制特效数量

### 4. 网络同步
- 使用预测
- 实现平滑插值
- 优化同步频率
- 压缩同步数据 