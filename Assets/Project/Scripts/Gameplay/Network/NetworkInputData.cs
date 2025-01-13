using Fusion;
using UnityEngine;

/// <summary>
/// Network input data structure for player actions
/// </summary>
public struct NetworkInputData : INetworkInput
{
    public float HorizontalInput;
    public float VerticalInput;
    public NetworkBool ShootPressed;
    public float ShootAngle;
}
