using System;
using System.Collections.Generic;

namespace Core.FSM
{
    public class StateMachine
    {
        private Dictionary<Type, IState> _states = new Dictionary<Type, IState>();
        private IState _currentState;
        private IState _previousState;

        public void AddState(IState state)
        {
            var stateType = state.GetType();
            if (!_states.ContainsKey(stateType))
            {
                _states[stateType] = state;
            }
        }

        public void ChangeState<T>() where T : IState
        {
            var newStateType = typeof(T);
            if (_states.TryGetValue(newStateType, out IState newState))
            {
                _currentState?.Exit();
                _previousState = _currentState;
                _currentState = newState;
                _currentState.Enter();
            }
        }

        public void Update()
        {
            _currentState?.Update();
        }

        public void RevertToPreviousState()
        {
            if (_previousState != null)
            {
                _currentState?.Exit();
                _currentState = _previousState;
                _currentState.Enter();
            }
        }

        public T GetState<T>() where T : IState
        {
            return _states.TryGetValue(typeof(T), out IState state) ? (T)state : default;
        }
    }
}
