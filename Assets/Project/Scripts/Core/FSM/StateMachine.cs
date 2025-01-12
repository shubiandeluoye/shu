using System.Collections.Generic;
using UnityEngine;
using Core.FSM;

namespace Core.FSM
{
    /// <summary>
    /// 有限状态机实现
    /// 用于管理游戏状态，支持状态切换和历史记录
    /// </summary>
    public class StateMachine
    {
        private IState _currentState;
        private Dictionary<System.Type, IState> _states = new Dictionary<System.Type, IState>();
        private Stack<IState> _stateHistory = new Stack<IState>();

        /// <summary>
        /// 添加一个新状态到状态机
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="state">状态实例</param>
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

        /// <summary>
        /// 切换到指定类型的状态
        /// </summary>
        /// <typeparam name="T">目标状态类型</typeparam>
        public void ChangeState<T>() where T : IState
        {
            var type = typeof(T);
            if (_states.TryGetValue(type, out IState newState))
            {
                if (_currentState != null)
                {
                    _currentState.Exit();
                    _stateHistory.Push(_currentState);
                }
                _currentState = newState;
                _currentState.Enter();
            }
            else
            {
                Debug.LogError($"[StateMachine] State {type} not found.");
            }
        }

        /// <summary>
        /// 更新当前状态
        /// </summary>
        public void Update()
        {
            _currentState?.Update();
        }

        /// <summary>
        /// 返回到上一个状态
        /// </summary>
        public void RevertToPreviousState()
        {
            if (_stateHistory.Count > 0)
            {
                if (_currentState != null)
                {
                    _currentState.Exit();
                }
                _currentState = _stateHistory.Pop();
                _currentState?.Enter();
            }
        }

        /// <summary>
        /// 获取指定类型的状态
        /// </summary>
        /// <typeparam name="T">要获取的状态类型</typeparam>
        /// <returns>状态实例，如果不存在则返回默认值</returns>
        public T GetState<T>() where T : IState
        {
            var type = typeof(T);
            return _states.TryGetValue(type, out IState state) ? (T)state : default;
        }
    }
}
