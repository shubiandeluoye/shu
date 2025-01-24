# Skill 模块说明文档

## 相关代码
### 核心系统
- [SkillSystemManager.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/SkillSystemManager.cs) - 技能系统总管理器
- [BaseSkill.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Core/BaseSkill.cs) - 技能基类
- [PassiveSkillBase.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Core/PassiveSkillBase.cs) - 被动技能基类
- [SkillConfig.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Core/SkillConfig.cs) - 技能配置
- [SkillContext.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Core/SkillContext.cs) - 技能上下文
- [SkillManager.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Core/SkillManager.cs) - 技能管理器

### 效果系统
- [BaseEffect.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Effects/BaseEffect.cs) - 效果基类
- [AreaEffect.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Effects/AreaEffect.cs) - 区域效果
- [BarrierEffect.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Effects/BarrierEffect.cs) - 屏障效果
- [HealEffect.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Effects/HealEffect.cs) - 治疗效果
- [ProjectileEffect.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Effects/ProjectileEffect.cs) - 投射物效果
- [EffectController.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Effects/EffectController.cs) - 效果控制器

### 具体技能
#### 屏障技能
- [BarrierSkill.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Skills/BarrierSkill/BarrierSkill.cs) - 屏障技能实现
- [BarrierConfig.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Skills/BarrierSkill/BarrierConfig.cs) - 屏障配置
- [BarrierBehaviour.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Skills/BarrierSkill/BarrierBehaviour.cs) - 屏障行为

#### 盒子技能
- [BoxSkill.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Skills/BoxSkill/BoxSkill.cs) - 盒子技能实现
- [BoxConfig.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Skills/BoxSkill/BoxConfig.cs) - 盒子配置
- [BoxBehaviour.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Skills/BoxSkill/BoxBehaviour.cs) - 盒子行为

#### 治疗技能
- [HealSkill.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Skills/HealSkill/HealSkill.cs) - 治疗技能实现
- [HealConfig.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Skills/HealSkill/HealConfig.cs) - 治疗配置

#### 射击技能
- [ShootSkill.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Skills/ShootSkill/ShootSkill.cs) - 射击技能实现
- [ShootConfig.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Skills/ShootSkill/ShootConfig.cs) - 射击配置
- [BulletController.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Skills/ShootSkill/BulletController.cs) - 子弹控制器

### 工具类
- [SkillUtils.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Utils/SkillUtils.cs) - 技能工具类
- [SkillAudioUtils.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Utils/SkillAudioUtils.cs) - 技能音效工具
- [SkillEffectUtils.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Utils/SkillEffectUtils.cs) - 技能特效工具

### 事件系统
- [SkillEvents.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Events/SkillEvents.cs) - 技能事件
- [EffectEvents.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/SkillModule/Events/EffectEvents.cs) - 效果事件

## 技能配置详解

### 1. 基础技能配置 (SkillConfig)
```csharp
[CreateAssetMenu(fileName = "NewSkillConfig", menuName = "Game/Skills/New Skill Config")]
public class SkillConfig : ScriptableObject
{
    public int SkillId;                 // 技能ID
    public string SkillName;            // 技能名称
    public float Cooldown;              // 冷却时间
    public float Duration;              // 持续时间
    public float Range;                 // 作用范围
    public SkillType Type;              // 技能类型
    public GameObject EffectPrefab;     // 效果预制体
    public AudioClip ActivateSound;     // 激活音效
}
```

### 2. 屏障技能配置 (BarrierConfig)
```csharp
public class BarrierConfig : SkillConfig
{
    public float BarrierHealth;         // 屏障生命值
    public float ReflectDamage;         // 反伤伤害
    public float BarrierRadius;         // 屏障半径
    public Material BarrierMaterial;    // 屏障材质
}
```

### 3. 射击技能配置 (ShootConfig)
```csharp
public class ShootConfig : SkillConfig
{
    public float ProjectileSpeed;       // 子弹速度
    public float ProjectileDamage;      // 子弹伤害
    public float SpreadAngle;           // 扩散角度
    public int ProjectileCount;         // 子弹数量
    public GameObject BulletPrefab;     // 子弹预制体
}
```

## 创建新技能流程

### 1. 创建配置文件
1. 右键Project窗口 -> Create -> Game -> Skills -> New Skill Config
2. 设置基础参数：
   - SkillId: 唯一技能ID
   - SkillName: 技能名称
   - Cooldown: 冷却时间
   - Duration: 持续时间
   - Range: 作用范围
   - Type: 技能类型
   - EffectPrefab: 效果预制体
   - ActivateSound: 激活音效

### 2. 创建技能类
```csharp
public class CustomSkill : BaseSkill
{
    private CustomConfig config;
    
    protected override void OnInitialize()
    {
        config = (CustomConfig)SkillConfig;
        // 初始化其他参数
    }

    protected override void OnActivate()
    {
        // 1. 检查前置条件
        if (!CanActivate()) return;

        // 2. 创建效果
        var effect = EffectController.CreateEffect<CustomEffect>();
        effect.Initialize(config);

        // 3. 应用效果
        effect.Apply(Target);

        // 4. 播放音效和特效
        SkillAudioUtils.PlaySkillSound(config.ActivateSound);
        SkillEffectUtils.PlaySkillEffect(config.EffectPrefab, Target.position);

        // 5. 开始冷却
        StartCooldown();
    }

    protected override void OnDeactivate()
    {
        // 清理效果
        effect?.Remove();
    }
}
```

### 3. 创建效果类
```csharp
public class CustomEffect : BaseEffect
{
    private CustomConfig config;
    
    public override void Initialize(EffectConfig config)
    {
        this.config = config;
        // 初始化效果参数
    }

    public override void Apply(GameObject target)
    {
        // 1. 应用效果逻辑
        // 2. 触发效果事件
        EventManager.Instance.TriggerEvent(new EffectAppliedEvent 
        { 
            EffectId = config.EffectId,
            Target = target
        });
    }

    public override void Remove()
    {
        // 1. 清理效果
        // 2. 触发移除事件
        EventManager.Instance.TriggerEvent(new EffectRemovedEvent
        {
            EffectId = config.EffectId
        });
    }
}
```

## 技能系统使用示例

### 1. 注册技能
```csharp
// 在PlayerManager或其他管理器中
void Start()
{
    // 1. 加载配置
    var config = Resources.Load<SkillConfig>("Skills/CustomSkill");
    
    // 2. 创建技能实例
    var skill = new CustomSkill();
    skill.Initialize(config);
    
    // 3. 注册技能
    SkillManager.Instance.RegisterSkill(skill);
}
```

### 2. 使用技能
```csharp
// 在Player或其他使用技能的组件中
void Update()
{
    // 1. 检查输入
    if (Input.GetKeyDown(KeyCode.Q))
    {
        // 2. 获取技能
        var skill = SkillManager.Instance.GetSkill<CustomSkill>();
        
        // 3. 激活技能
        if (skill.CanActivate())
        {
            skill.Activate();
        }
    }
}
```

### 3. 监听技能事件
```csharp
void OnEnable()
{
    // 注册事件监听
    EventManager.Instance.AddListener<SkillActivatedEvent>(OnSkillActivated);
    EventManager.Instance.AddListener<SkillEndedEvent>(OnSkillEnded);
}

void OnDisable()
{
    // 移除事件监听
    EventManager.Instance.RemoveListener<SkillActivatedEvent>(OnSkillActivated);
    EventManager.Instance.RemoveListener<SkillEndedEvent>(OnSkillEnded);
}

private void OnSkillActivated(SkillActivatedEvent evt)
{
    // 处理技能激活事件
    Debug.Log($"技能 {evt.SkillId} 已激活");
}

private void OnSkillEnded(SkillEndedEvent evt)
{
    // 处理技能结束事件
    Debug.Log($"技能 {evt.SkillId} 已结束");
}
```

## 禁止修改项
1. 核心接口和基类
   - BaseSkill
   - BaseEffect
   - SkillManager
   - EffectController

2. 事件系统
   - SkillEvents
   - EffectEvents
   - 事件触发机制

3. 网络同步
   - RPC调用机制
   - 状态同步逻辑
   - 权限验证

4. 配置系统
   - ScriptableObject基类
   - 配置加载机制
   - 参数验证逻辑

## 可以修改项
1. 技能实现
   - 具体技能类
   - 效果实现
   - 技能组合
   - 触发条件

2. 配置参数
   - 技能数值
   - 效果参数
   - 音效和特效
   - 预制体引用

3. 表现效果
   - 特效系统
   - 音效系统
   - 动画系统
   - UI反馈

4. 扩展功能
   - 新技能类型
   - 新效果类型
   - 自定义事件
   - 额外参数

## 常见问题解决
1. 技能无法释放
   - 检查技能ID是否正确注册
   - 验证冷却时间是否结束
   - 确认技能条件是否满足
   - 检查网络权限是否正确

2. 效果不显示
   - 确认预制体是否正确加载
   - 检查效果参数是否正确
   - 验证特效系统是否正常
   - 确认场景相机设置

3. 网络同步问题
   - 检查NetworkObject组件
   - 确认RPC调用权限
   - 验证网络状态
   - 检查同步参数

4. 性能问题
   - 使用对象池管理效果
   - 优化特效粒子数量
   - 合理设置更新频率
   - 使用GPU Instancing 