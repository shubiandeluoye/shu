using UnityEngine;

/// <summary>
/// Interface defining the contract for state implementations
/// </summary>
public interface IState
{
    void Enter();
    void Update();
    void Exit();
}
