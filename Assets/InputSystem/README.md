# Input System 模块说明文档

## 关于 GameInputActions
这是由 Unity 的新输入系统自动生成的代码文件，基于 GameInputActions.inputactions 配置文件生成。请注意：
- 不要直接修改 GameInputActions.cs 文件
- 所有修改都应该在 GameInputActions.inputactions 中进行
- 修改后 Unity 会自动重新生成代码文件

## 当前输入配置

### Player 输入映射
1. 移动控制 (Move)
   - W/S - 上下移动
   - A/D - 左右移动
   - 类型：Value
   - 控制类型：2D Vector

2. 射击控制
   - J - 直线射击 (StraightShoot)
   - N - 左角度射击 (LeftAngleShoot)
   - K - 右角度射击 (RightAngleShoot)
   - M - 切换角度 (ToggleAngle)
   - 类型：Button

3. 子弹控制
   - Q - 切换子弹等级 (ToggleBulletLevel)
   - E - 发射3级子弹 (FireLevel3Bullet)
   - 类型：Button

### Touch 输入映射
1. 主要触摸位置 (PrimaryFingerPosition)
   - 类型：Value
   - 控制类型：Vector2

2. 次要触摸位置 (SecondaryFingerPosition)
   - 类型：Value
   - 控制类型：Vector2

## 使用方法

### 1. 初始化输入系统
```csharp
private GameInputActions _inputActions;

void Awake()
{
    _inputActions = new GameInputActions();
    _inputActions.Enable();
}

void OnDestroy()
{
    _inputActions.Disable();
}
```

### 2. 监听输入事件
```csharp
// 监听移动输入
_inputActions.Player.Move.performed += OnMove;
_inputActions.Player.Move.canceled += OnMoveEnd;

// 监听射击输入
_inputActions.Player.StraightShoot.performed += OnStraightShoot;
```

### 3. 读取输入值
```csharp
// 读取移动向量
Vector2 moveInput = _inputActions.Player.Move.ReadValue<Vector2>();

// 读取触摸位置
Vector2 touchPos = _inputActions.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
```

## 修改输入配置

1. 打开输入配置文件
   - 在 Project 窗口找到 GameInputActions.inputactions
   - 双击打开 Input Actions 编辑器

2. 添加新的输入动作
   - 选择合适的 Action Map
   - 点击 + 添加新的 Action
   - 配置动作类型和参数
   - 添加输入绑定

3. 修改现有输入
   - 选择要修改的 Action
   - 调整参数或绑定
   - Unity 会自动重新生成代码

## 注意事项

1. 代码生成
   - 修改 .inputactions 文件后会自动重新生成代码
   - 确保生成的代码文件在正确的命名空间下
   - 不要手动修改生成的代码

2. 性能考虑
   - 及时启用/禁用不需要的 Action Map
   - 合理使用 performed/started/canceled 事件
   - 避免在 Update 中频繁读取输入值

3. 多平台支持
   - 确保为不同平台配置合适的输入绑定
   - 测试所有目标平台的输入响应
   - 考虑不同设备的输入特点

4. 调试方法
   - 使用 Unity 的输入调试器
   - 检查输入事件的触发
   - 验证输入值的范围 