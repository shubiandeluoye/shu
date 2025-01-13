using Fusion;
using UnityEngine;

/// <summary>
/// Network input data structure for Fusion state synchronization
/// </summary>
public struct NetworkInputData : INetworkInput
{
    public float HorizontalInput;
    public float VerticalInput;
    public bool ShootPressed;
    public float ShootAngle;
    public NetworkButtons Buttons;
}
