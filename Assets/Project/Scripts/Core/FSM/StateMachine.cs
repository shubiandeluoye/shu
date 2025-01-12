using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Finite State Machine implementation for managing game states
/// </summary>
public class StateMachine
{
    private IState _currentState;
    private Dictionary<System.Type, IState> _states = new Dictionary<System.Type, IState>();
    private Stack<IState> _stateHistory = new Stack<IState>();

    public void AddState<T>(T state) where T : IState
    {
        var type = typeof(T);
        if (!_states.ContainsKey(type))
        {
            _states[type] = state;
        }
        else
        {
            Debug.LogWarning($"[StateMachine] State {type} already exists.");
        }
    }

    public void ChangeState<T>() where T : IState
    {
        var type = typeof(T);
        if (_states.TryGetValue(type, out IState newState))
        {
            _currentState?.Exit();
            _stateHistory.Push(_currentState);
            _currentState = newState;
            _currentState.Enter();
        }
        else
        {
            Debug.LogError($"[StateMachine] State {type} not found.");
        }
    }

    public void Update()
    {
        _currentState?.Update();
    }

    public void RevertToPreviousState()
    {
        if (_stateHistory.Count > 0)
        {
            _currentState?.Exit();
            _currentState = _stateHistory.Pop();
            _currentState.Enter();
        }
    }

    public T GetState<T>() where T : IState
    {
        var type = typeof(T);
        if (_states.TryGetValue(type, out IState state))
        {
            return (T)state;
        }
        return default;
    }
}
