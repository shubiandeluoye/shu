using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

/// <summary>
/// Handles input processing and management for both PC and mobile platforms
/// </summary>
public class InputManager : Singleton<InputManager>
{
    // Input action events
    public event Action<Vector2> OnMove;
    public event Action OnShootSmall;
    public event Action OnShootMedium;
    public event Action OnShootLarge;
    public event Action OnAngleChange;
    public event Action<int> OnDirectionChange; // -1 left, 0 straight, 1 right

    private PlayerInput playerInput;
    private Dictionary<string, InputActionMap> actionMaps;

    protected override void Awake()
    {
        base.Awake();
        InitializeInput();
    }

    private void InitializeInput()
    {
        // Create input action maps for different control schemes
        actionMaps = new Dictionary<string, InputActionMap>();
        
        // PC Controls
        var pcMap = new InputActionMap("PCControls");
        
        // Movement (WASD)
        pcMap.AddAction("Move", binding: "<Keyboard>/w,<Keyboard>/s,<Keyboard>/a,<Keyboard>/d");
        
        // Shooting (J/K/N)
        pcMap.AddAction("ShootStraight", binding: "<Keyboard>/j");
        pcMap.AddAction("ShootLeft", binding: "<Keyboard>/k");
        pcMap.AddAction("ShootRight", binding: "<Keyboard>/n");
        
        // Bullet type (Q/E)
        pcMap.AddAction("SwitchBullet", binding: "<Keyboard>/q,<Keyboard>/e");
        
        // Angle change (M)
        pcMap.AddAction("ChangeAngle", binding: "<Keyboard>/m");

        actionMaps["PC"] = pcMap;

        // Mobile Controls (using Input System's on-screen controls)
        var mobileMap = new InputActionMap("MobileControls");
        mobileMap.AddAction("Move", binding: "<Gamepad>/leftStick");
        // Add other mobile-specific controls...

        actionMaps["Mobile"] = mobileMap;

        // Enable PC controls by default
        SetControlScheme("PC");
    }

    public void SetControlScheme(string scheme)
    {
        foreach (var map in actionMaps.Values)
        {
            map.Disable();
        }

        if (actionMaps.TryGetValue(scheme, out var actionMap))
        {
            actionMap.Enable();
        }
    }

    private void OnEnable()
    {
        // Subscribe to input events
        if (actionMaps.TryGetValue("PC", out var pcMap))
        {
            pcMap["Move"].performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
            pcMap["ShootStraight"].performed += ctx => OnDirectionChange?.Invoke(0);
            pcMap["ShootLeft"].performed += ctx => OnDirectionChange?.Invoke(-1);
            pcMap["ShootRight"].performed += ctx => OnDirectionChange?.Invoke(1);
            pcMap["ChangeAngle"].performed += ctx => OnAngleChange?.Invoke();
        }
    }

    private void OnDisable()
    {
        foreach (var map in actionMaps.Values)
        {
            map.Disable();
        }
    }

    public bool IsActionPressed(string actionName, string scheme = "PC")
    {
        if (actionMaps.TryGetValue(scheme, out var actionMap))
        {
            return actionMap[actionName].IsPressed();
        }
        return false;
    }
}
