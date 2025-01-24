# GlassBreaking 模块说明文档

## 相关代码

### 核心系统
- [GlassBreakingManager.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/GlassBreaking/GlassBreakingManager.cs)
  - 玻璃破碎系统总管理器
  - 管理破碎效果生命周期
  - 协调物理模拟
  - 控制碎片生成

### Core/ - 核心功能
- [GlassPhysics.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/GlassBreaking/Core/GlassPhysics.cs)
  - 玻璃物理系统
  - 碎片物理模拟
  - 碰撞检测
  - 力的传递计算

- [GlassFragmentGenerator.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/GlassBreaking/Core/GlassFragmentGenerator.cs)
  - 碎片生成系统
  - 破碎模式计算
  - 碎片形状生成
  - 碎片分布控制

## 资源文件

### Materials/
- [GlassMaterial.mat](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Materials/Glass/GlassMaterial.mat)
  - 玻璃材质
  - 透明度设置
  - 反射属性
  - 折射效果

- [GlassPhysicsMaterial.physicmaterial](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Materials/Glass/GlassPhysicsMaterial.physicmaterial)
  - 玻璃物理材质
  - 摩擦系数
  - 弹性系数
  - 碰撞属性

### Prefabs/
- [GlassBreakingPrefab.prefab](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Prefabs/Glass/GlassBreakingPrefab.prefab)
  - 玻璃破碎预制体
  - 完整玻璃模型
  - 碎片预设
  - 特效系统

- [GlassFragmentLarge.prefab](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Prefabs/Glass/GlassFragmentLarge.prefab)
- [GlassFragmentMedium.prefab](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Prefabs/Glass/GlassFragmentMedium.prefab)
- [GlassFragmentSmall.prefab](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Prefabs/Glass/GlassFragmentSmall.prefab)
  - 不同尺寸的碎片预制体
  - 碎片碰撞体
  - 物理属性
  - 渲染设置

- [GlassParticleSystem.prefab](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Prefabs/Glass/GlassParticleSystem.prefab)
  - 玻璃破碎粒子系统
  - 碎片粒子效果
  - 灰尘效果
  - 闪光效果

## 配置详解

### 1. 破碎配置
```csharp
[System.Serializable]
public class GlassBreakConfig
{
    public float MinImpactForce = 5f;      // 最小破碎力
    public float MaxFragments = 50;        // 最大碎片数
    public float FragmentLifetime = 5f;    // 碎片存活时间
    public float ShatterRadius = 1f;       // 破碎半径
    public bool UsePhysics = true;         // 启用物理
    public BreakPattern BreakPattern;      // 破碎模式
}

public enum BreakPattern
{
    Radial,         // 放射状
    Spider,         // 蜘蛛网状
    Shatter,        // 碎裂
    Custom          // 自定义
}
```

### 2. 物理配置
```csharp
[System.Serializable]
public class GlassPhysicsConfig
{
    public float Density = 2.5f;           // 密度
    public float Friction = 0.3f;          // 摩擦力
    public float Restitution = 0.3f;       // 弹性
    public float LinearDrag = 0.1f;        // 线性阻力
    public float AngularDrag = 0.1f;       // 角阻力
    public bool UseGravity = true;         // 使用重力
}
```

## 修改限制

### 禁止修改
1. 核心破碎算法
   - 碎片生成算法
   - 物理模拟核心
   - 碰撞检测系统
   - 这些影响破碎效果的基础实现

2. 渲染管线集成
   - 材质渲染管线
   - 透明度处理
   - 反射计算
   - 这些影响视觉效果的核心部分

3. 性能优化核心
   - 对象池系统
   - 碎片合并逻辑
   - LOD系统
   - 这些影响性能的关键部分

### 可以修改
1. 视觉参数
   - 材质属性
   - 碎片大小
   - 粒子效果
   - 破碎模式

2. 物理参数
   - 力的阈值
   - 碎片数量
   - 物理属性
   - 存活时间

3. 特效设置
   - 粒子系统
   - 声音效果
   - 后处理效果
   - 环境交互

### 需要审核的修改
1. 破碎算法优化
   - 新的破碎模式
   - 碎片生成算法
   - 物理模拟改进
   - 需要性能测试

2. 渲染优化
   - 新的渲染技术
   - 材质系统改进
   - 性能优化方案
   - 需要视觉效果评估

## 使用示例

### 1. 基础破碎效果
```csharp
public class GlassBreakExample : MonoBehaviour
{
    [SerializeField] private GlassBreakConfig breakConfig;
    private GlassBreakingManager glassManager;

    public void Break(Vector3 impactPoint, float force)
    {
        if (force >= breakConfig.MinImpactForce)
        {
            glassManager.BreakGlass(impactPoint, force, breakConfig);
        }
    }
}
```

### 2. 自定义破碎模式
```csharp
public class CustomBreakPattern : MonoBehaviour
{
    [SerializeField] private GlassBreakConfig config;
    private GlassFragmentGenerator generator;

    public void GenerateCustomPattern(Vector3 center)
    {
        var pattern = new BreakPattern();
        pattern.GenerateCustomPattern(center, config);
        generator.ApplyPattern(pattern);
    }
}
```

## 性能优化

### 1. 渲染优化
- 使用GPU Instancing
- 实现LOD系统
- 优化材质复杂度
- 控制最大碎片数

### 2. 物理优化
- 使用对象池
- 简化碰撞体
- 优化物理更新
- 合并小碎片

### 3. 内存优化
- 复用碎片网格
- 共享材质实例
- 优化贴图内存
- 及时清理碎片

## 调试工具

### 1. 性能监控
```csharp
public class GlassBreakingDebugger : MonoBehaviour
{
    [SerializeField] private bool showPerformanceStats = true;
    [SerializeField] private bool logPhysicsEvents = true;

    private void LogPerformance()
    {
        if (showPerformanceStats)
        {
            Debug.Log($"Active Fragments: {activeFragments}\n" +
                     $"Physics Time: {physicsTime}ms\n" +
                     $"Draw Calls: {drawCalls}");
        }
    }
}
```

### 2. 视觉调试
```csharp
public class GlassVisualDebugger : MonoBehaviour
{
    [SerializeField] private bool showBreakPattern = true;
    [SerializeField] private bool showColliders = true;

    private void OnDrawGizmos()
    {
        if (showBreakPattern)
        {
            DrawBreakPattern();
        }
        if (showColliders)
        {
            DrawColliders();
        }
    }
}
```

## 常见问题解决

### 1. 性能问题
- 降低碎片数量
- 简化物理计算
- 优化渲染设置
- 使用LOD系统

### 2. 视觉问题
- 调整材质参数
- 检查光照设置
- 优化碎片分布
- 改进特效表现

### 3. 物理问题
- 检查碰撞设置
- 调整物理参数
- 优化力的传递
- 处理穿透问题

## 维护建议

### 1. 定期维护
- 检查性能指标
- 更新优化方案
- 清理无用资源
- 更新调试工具

### 2. 版本更新
- 测试新功能
- 验证兼容性
- 更新文档
- 备份配置
