# Game Module 文档

## 1. 模块结构

### 1.1 核心类
- `GameManager.cs`
  - 位置：Assets/Project/Scripts/GameModule/
  - 功能：游戏系统总管理器，模块唯一对外接口
  - 依赖：GameStateManager, MatchManager, ScoreManager

- `GameStateManager.cs`
  - 位置：Assets/Project/Scripts/GameModule/Core/
  - 功能：游戏状态管理，处理状态转换和倒计时
  - 依赖：StateConfig

- `MatchManager.cs`
  - 位置：Assets/Project/Scripts/GameModule/Core/
  - 功能：匹配系统，处理玩家加入/离开和准备状态
  - 依赖：MatchConfig

- `ScoreManager.cs`
  - 位置：Assets/Project/Scripts/GameModule/Core/
  - 功能：分数系统，处理得分、排名和统计
  - 依赖：ScoreConfig

### 1.2 数据类
- `GameConfig.cs`
  - 位置：Assets/Project/Scripts/GameModule/Data/
  - 功能：游戏配置数据
  - 关键配置：
    - StateConfig: 状态相关配置
    - MatchConfig: 匹配相关配置
    - ScoreConfig: 分数相关配置
    - RuleConfig: 规则相关配置

- `GameEvents.cs`
  - 位置：Assets/Project/Scripts/GameModule/Data/
  - 功能：定义所有游戏相关事件
  - 事件类型：
    - 游戏核心事件：
      - GameStartEvent: 游戏开始
      - GameEndEvent: 游戏结束
      - GamePauseEvent: 游戏暂停
      - GameStateChangeEvent: 状态变化
      - GameCountdownEvent: 倒计时
    - 玩家事件：
      - PlayerJoinEvent: 玩家加入
      - PlayerLeaveEvent: 玩家离开
      - PlayerDeathEvent: 玩家死亡
    - 匹配事件：
      - MatchmakingStartEvent: 开始匹配
      - MatchmakingStopEvent: 停止匹配
      - MatchReadyEvent: 匹配就绪
      - MatchTimeoutEvent: 匹配超时
      - PlayerCountChangeEvent: 玩家数量变化
    - 分数事件：
      - ScoreUpdateEvent: 分数更新
      - GameOverEvent: 游戏结束
    - 排行榜事件：
      - RankingChangeEvent: 排名变化

- `GameState.cs`
  - 位置：Assets/Project/Scripts/GameModule/Data/
  - 功能：游戏状态和枚举定义
  - 关键定义：
    - GameState: 游戏状态枚举
    - GameOverReason: 游戏结束原因
    - PlayerScore: 玩家分数数据

## 2. 网络同步说明
所有需要同步的状态都使用 [Networked] 属性标记：
- IsGameStarted: 游戏是否开始
- IsGamePaused: 游戏是否暂停
- GameTime: 游戏时间

## 3. 主要功能
1. 游戏流程控制
   - 游戏开始/结束
   - 暂停/继续
   - 状态转换
   - 倒计时系统

2. 匹配系统
   - 玩家加入/离开管理
   - 准备状态管理
   - 匹配超时处理
   - 人数检查

3. 分数系统
   - 得分计算
   - 击杀统计
   - 生存时间奖励
   - 排行榜管理

4. 规则系统
   - 时间限制
   - 胜利条件判定
   - 玩家数量检查
   - 重生机制

## 4. 注意事项
1. 所有状态更新需要检查 StateAuthority
2. 事件系统用于模块间通信，所有事件定义统一在 GameEvents.cs 中
3. 配置值需要在 Inspector 中设置
4. 玩家生成需要地图模块配合
5. 分数系统会影响游戏结束判定 