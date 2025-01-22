# 玻璃破碎系统

一个灵活的 Unity 玻璃破碎效果系统，具有真实的物理效果、可自定义的破碎模式和优化的性能表现。

## 功能特性

- 透明度控制（0-1范围）
- 多种破碎模式（圆形/线性）
- 基于物理的碎片行为
- 粒子特效
- 移动平台性能优化
- 对象池系统实现高效内存使用

## 使用方法

1. 将 GlassBreakingPrefab 添加到场景中
2. 配置 GlassBreakingController 组件：
   - 设置透明度（0-1）
   - 选择破碎模式（圆形/线性）
   - 调整破碎半径和碎片数量
   - 配置性能设置

示例代码：
```csharp
// 获取控制器引用
var glassController = GetComponent<GlassBreakingController>();

// 设置属性
glassController.transparency = 0.8f;
glassController.useCircularBreak = true;
glassController.breakRadius = 1.5f;

// 触发破碎
glassController.BreakGlass();
```

## 性能设置

- PC平台：
  * 最大碎片数：50
  * 剔除距离：10米
  * 完整物理模拟

- 移动平台：
  * 最大碎片数：35
  * 剔除距离：6-8米
  * 简化物理计算
  * LOD系统启用

## 系统要求

- Unity 2022.3.LTS
- URP 14.0.10
- DOTween（免费版）
