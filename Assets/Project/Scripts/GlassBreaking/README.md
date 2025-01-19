# Glass Breaking System

A flexible glass breaking effect system for Unity, featuring realistic physics, customizable break patterns, and optimized performance.

## Features

- Transparency control (0-1 range)
- Multiple break patterns (circular/linear)
- Physics-based fragment behavior
- Particle effects
- Performance optimizations for mobile
- Object pooling for efficient memory usage

## Usage

1. Add the GlassBreakingPrefab to your scene
2. Configure the GlassBreakingController component:
   - Set transparency level (0-1)
   - Choose break pattern (circular/linear)
   - Adjust break radius and fragment count
   - Configure performance settings

Example:
```csharp
// Get reference to controller
var glassController = GetComponent<GlassBreakingController>();

// Set properties
glassController.transparency = 0.8f;
glassController.useCircularBreak = true;
glassController.breakRadius = 1.5f;

// Trigger break
glassController.BreakGlass();
```

## Performance Settings

- PC Platform:
  * Maximum fragments: 50
  * Culling distance: 10m
  * Full physics simulation

- Mobile Platform:
  * Maximum fragments: 35
  * Culling distance: 6-8m
  * Simplified physics
  * LOD system active

## Requirements

- Unity 2022.3.LTS
- URP 14.0.10
- DOTween (Free version)
