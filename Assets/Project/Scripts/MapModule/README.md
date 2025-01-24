# Map 模块说明文档

## 相关代码

### 核心管理器
- [MapManager.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/MapModule/MapManager.cs)
  - 地图系统总管理器
  - 管理地图加载和卸载
  - 控制地图状态
  - 处理地图事件

### Core/ - 核心功能
- [MapGenerator.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/MapModule/Core/MapGenerator.cs)
  - 地图生成系统
  - 程序化地图生成
  - 地形生成
  - 障碍物放置

- [MapLoader.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/MapModule/Core/MapLoader.cs)
  - 地图加载系统
  - 资源加载管理
  - 场景切换控制
  - 加载进度处理

### Systems/ - 功能系统
- [SpawnPointSystem.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/MapModule/Systems/SpawnPointSystem.cs)
  - 出生点系统
  - 出生点分配
  - 队伍出生点管理
  - 重生点选择

- [CoverSystem.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/MapModule/Systems/CoverSystem.cs)
  - 掩体系统
  - 掩体检测
  - 掩护点计算
  - 战术位置分析

## 配置详解

### 1. 地图配置
```csharp
[System.Serializable]
public class MapConfig
{
    public string MapName = "Default";      // 地图名称
    public Vector2 MapSize = new Vector2(100f, 100f); // 地图大小
    public int MaxPlayers = 10;             // 最大玩家数
    public MapType MapType;                 // 地图类型
    public bool UseRandomGeneration;        // 是否使用随机生成
    public float CoverDensity = 0.3f;      // 掩体密度
}

public enum MapType
{
    Arena,          // 竞技场
    Urban,          // 城市
    Wilderness,     // 荒野
    Custom          // 自定义
}
```

### 2. 生成配置
```csharp
[System.Serializable]
public class MapGenerationConfig
{
    public float TerrainHeight = 10f;       // 地形高度
    public float NoiseScale = 1f;           // 噪声比例
    public int Seed = 0;                    // 随机种子
    public bool GenerateCovers = true;      // 生成掩体
    public bool GenerateProps = true;       // 生成道具
}
```

## 修改限制

### 禁止修改
1. 核心生成算法
   - 地形生成核心
   - 随机数生成器
   - 资源加载系统
   - 这些影响地图生成的基础实现

2. 场景管理系统
   - 场景加载机制
   - 资源释放逻辑
   - 内存管理
   - 这些影响性能和稳定性的核心部分

3. 网络同步相关
   - 地图状态同步
   - 出生点分配逻辑
   - 这些影响多人游戏体验的关键部分

### 可以修改
1. 地图参数
   - 地图大小设置
   - 地形参数
   - 掩体分布
   - 道具位置

2. 视觉效果
   - 地形材质
   - 环境效果
   - 光照设置
   - 后处理效果

3. 游戏性设置
   - 出生点布局
   - 掩体分布
   - 道具生成
   - 地图平衡性

### 需要审核的修改
1. 生成算法优化
   - 新的生成方法
   - 性能优化方案
   - 内存使用优化
   - 需要完整测试

2. 玩法修改
   - 地图机制变更
   - 交互系统改动
   - 平衡性调整
   - 需要游戏设计评估

## 使用示例

### 1. 加载地图
```csharp
public class MapLoadExample : MonoBehaviour
{
    [SerializeField] private MapConfig mapConfig;
    private MapLoader mapLoader;

    public async Task LoadMap()
    {
        var progress = new Progress<float>();
        progress.ProgressChanged += HandleProgress;
        
        await mapLoader.LoadMapAsync(mapConfig.MapName, progress);
    }

    private void HandleProgress(float progress)
    {
        Debug.Log($"Loading Progress: {progress * 100}%");
    }
}
```

### 2. 生成地图
```csharp
public class MapGenerationExample : MonoBehaviour
{
    [SerializeField] private MapGenerationConfig genConfig;
    private MapGenerator generator;

    public void GenerateMap()
    {
        generator.SetSeed(genConfig.Seed);
        generator.GenerateTerrain(genConfig);
        
        if (genConfig.GenerateCovers)
        {
            generator.GenerateCovers();
        }
    }
}
```

## 调试工具

### 1. 地图调试器
```csharp
public class MapDebugger : MonoBehaviour
{
    [SerializeField] private bool showSpawnPoints = true;
    [SerializeField] private bool showCoverPoints = true;
    [SerializeField] private bool showGridLines = true;

    private void OnDrawGizmos()
    {
        if (showSpawnPoints) DrawSpawnPoints();
        if (showCoverPoints) DrawCoverPoints();
        if (showGridLines) DrawGrid();
    }
}
```

### 2. 性能监控
```csharp
public class MapPerformanceMonitor : MonoBehaviour
{
    [SerializeField] private bool logLoadTimes = true;
    [SerializeField] private bool monitorMemory = true;

    private void LogMapStats()
    {
        if (logLoadTimes)
        {
            Debug.Log($"Load Time: {loadTime}ms\n" +
                     $"Memory Used: {memoryUsed}MB\n" +
                     $"Draw Calls: {drawCalls}");
        }
    }
}
```

## 常见问题解决

### 1. 加载问题
- 检查资源完整性
- 验证内存使用
- 优化加载顺序
- 处理加载失败

### 2. 生成问题
- 检查生成参数
- 验证随机种子
- 优化生成算法
- 处理边界情况

### 3. 性能问题
- 优化资源使用
- 实现LOD系统
- 控制绘制调用
- 优化碰撞检测

## 维护建议

### 1. 定期维护
- 检查资源完整性
- 优化生成参数
- 更新调试工具
- 清理未使用资源

### 2. 版本更新
- 测试新地图
- 验证生成系统
- 更新文档
- 备份配置 