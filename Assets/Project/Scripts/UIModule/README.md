# UI 模块说明文档

## 相关代码

### 核心系统
- [UIModuleManager.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/UIModule/Core/UIModuleManager.cs)
  - UI系统总管理器
  - 管理UI生命周期
  - 控制UI层级
  - 处理UI事件

### Core/ - 核心功能
- [UIBase.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/UIModule/Core/UIBase.cs)
  - UI基类
  - 定义UI基础行为
  - 提供通用功能

- [UIModule.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/UIModule/Core/UIModule.cs)
  - UI模块基类
  - 定义模块接口
  - 管理模块生命周期

### Components/ - UI组件
- [ToastUI.cs](https://github.com/shubiandeluoye/shu/blob/my-new-branch/Assets/Project/Scripts/UIModule/Components/ToastUI.cs)
  - Toast提示组件
  - 显示临时提示
  - 管理提示队列

## UI配置详解

### 1. UI基础配置
```csharp
[System.Serializable]
public class UIConfig
{
    public string UIName;           // UI名称
    public int SortingOrder;        // 渲染顺序
    public UILayer Layer;           // UI层级
    public bool IsPersistent;       // 是否常驻
    public bool UseAnimation;       // 是否使用动画
    public GameObject Prefab;       // UI预制体
}

public enum UILayer
{
    Background,     // 背景层
    Normal,         // 普通层
    PopUp,         // 弹出层
    Toast,         // 提示层
    Loading        // 加载层
}
```

### 2. Toast配置
```csharp
[System.Serializable]
public class ToastConfig
{
    public float Duration = 2f;     // 显示时长
    public float FadeTime = 0.5f;   // 淡出时间
    public Vector2 Offset;          // 位置偏移
    public TextAnchor Alignment;    // 对齐方式
}
```

## 使用示例

### 1. 创建新UI
```csharp
public class CustomUI : UIBase
{
    protected override void OnInitialize()
    {
        // 初始化UI
    }

    protected override void OnShow()
    {
        // UI显示时的逻辑
    }

    protected override void OnHide()
    {
        // UI隐藏时的逻辑
    }
}
```

### 2. 显示/隐藏UI
```csharp
// 显示UI
UIModuleManager.Instance.Show<CustomUI>();

// 隐藏UI
UIModuleManager.Instance.Hide<CustomUI>();
```

### 3. 显示Toast
```csharp
// 显示简单提示
ToastUI.Show("操作成功");

// 显示带配置的提示
var config = new ToastConfig
{
    Duration = 3f,
    FadeTime = 0.3f,
    Offset = new Vector2(0, 100)
};
ToastUI.Show("自定义提示", config);
```

## 禁止修改项

1. 核心接口和基类
   - UIBase
   - UIModule
   - UIModuleManager

2. 生命周期
   - 初始化流程
   - 显示/隐藏机制
   - 销毁流程

3. 层级系统
   - 层级定义
   - 排序机制
   - 渲染顺序

4. 事件系统
   - UI事件定义
   - 事件分发机制
   - 事件优先级

## 可以修改项

1. UI实现
   - 具体UI类
   - UI布局
   - UI交互
   - UI动画

2. 配置参数
   - UI样式
   - 动画参数
   - 音效配置
   - 预制体引用

3. 表现效果
   - 视觉风格
   - 动画效果
   - 过渡效果
   - 交互反馈

4. 扩展功能
   - 新UI组件
   - 自定义效果
   - 特殊交互
   - 额外功能

## 常见问题解决

1. UI不显示
   - 检查Canvas设置
   - 验证层级关系
   - 确认预制体加载
   - 检查显示条件

2. UI交互问题
   - 检查事件系统
   - 验证射线检测
   - 确认交互组件
   - 检查遮挡关系

3. 性能问题
   - 合理使用对象池
   - 控制重建频率
   - 优化渲染批次
   - 管理内存使用

4. 动画问题
   - 检查动画组件
   - 验证动画参数
   - 确认触发条件
   - 检查时序问题 