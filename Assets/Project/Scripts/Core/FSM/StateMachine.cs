using System.Collections.Generic;
using UnityEngine;
using Core.FSM;
using System.Linq;

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
        private Dictionary<System.Type, StateTransition> _transitions = new Dictionary<System.Type, StateTransition>();
        private float _stateTime;
        private bool _isTransitioning;

        public struct StateTransition
        {
            public System.Type FromState;
            public System.Type ToState;
            public System.Func<bool> Condition;
            public float TransitionDelay;
            public System.Action OnTransition;
        }

        public IState CurrentState => _currentState;
        public float CurrentStateTime => _stateTime;
        public bool IsTransitioning => _isTransitioning;

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
                Debug.Log($"[StateMachine] Added state: {type.Name}");
            }
            else
            {
                Debug.LogWarning($"[StateMachine] State {type.Name} already exists.");
            }
        }

        /// <summary>
        /// 添加一个状态转换到状态机
        /// </summary>
        /// <typeparam name="TFrom">源状态类型</typeparam>
        /// <typeparam name="TTo">目标状态类型</typeparam>
        /// <param name="condition">转换条件</param>
        /// <param name="delay">转换延迟</param>
        /// <param name="onTransition">转换完成后的回调</param>
        public void AddTransition<TFrom, TTo>(System.Func<bool> condition = null, float delay = 0f, System.Action onTransition = null)
            where TFrom : IState
            where TTo : IState
        {
            var fromType = typeof(TFrom);
            var toType = typeof(TTo);

            if (!_states.ContainsKey(fromType) || !_states.ContainsKey(toType))
            {
                Debug.LogError($"[StateMachine] Cannot add transition: states not found");
                return;
            }

            var transition = new StateTransition
            {
                FromState = fromType,
                ToState = toType,
                Condition = condition,
                TransitionDelay = delay,
                OnTransition = onTransition
            };

            var key = System.Tuple.Create(fromType, toType);
            _transitions[fromType] = transition;
            Debug.Log($"[StateMachine] Added transition: {fromType.Name} -> {toType.Name}");
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
                    if (_isTransitioning)
                    {
                        Debug.LogWarning($"[StateMachine] State change requested while transitioning");
                        return;
                    }

                    _currentState.Exit();
                    _stateHistory.Push(_currentState);
                    Debug.Log($"[StateMachine] Exited state: {_currentState.GetType().Name}");
                }

                _currentState = newState;
                _stateTime = 0f;
                _currentState.Enter();
                Debug.Log($"[StateMachine] Entered state: {type.Name}");
            }
            else
            {
                Debug.LogError($"[StateMachine] State {type.Name} not found.");
            }
        }

        /// <summary>
        /// 更新当前状态
        /// </summary>
        public void Update()
        {
            if (_currentState == null) return;

            _stateTime += Time.deltaTime;
            _currentState.Update();

            // 检查自动转换
            if (!_isTransitioning)
            {
                CheckTransitions();
            }
        }

        private void CheckTransitions()
        {
            var currentType = _currentState.GetType();
            if (_transitions.TryGetValue(currentType, out var transition))
            {
                if (transition.Condition?.Invoke() ?? true)
                {
                    StartTransition(transition);
                }
            }
        }

        private void StartTransition(StateTransition transition)
        {
            if (transition.TransitionDelay <= 0)
            {
                CompleteTransition(transition);
            }
            else
            {
                _isTransitioning = true;
                Debug.Log($"[StateMachine] Starting transition with delay: {transition.TransitionDelay}s");
                // 使用协程或定时器处理延迟
                // 这里简化处理，实际项目中可能需要更复杂的实现
                UnityEngine.MonoBehaviour.FindObjectOfType<MonoBehaviour>()?.StartCoroutine(
                    TransitionCoroutine(transition));
            }
        }

        private System.Collections.IEnumerator TransitionCoroutine(StateTransition transition)
        {
            yield return new WaitForSeconds(transition.TransitionDelay);
            CompleteTransition(transition);
        }

        private void CompleteTransition(StateTransition transition)
        {
            transition.OnTransition?.Invoke();
            var targetState = _states[transition.ToState];
            _currentState.Exit();
            _stateHistory.Push(_currentState);
            _currentState = targetState;
            _stateTime = 0f;
            _currentState.Enter();
            _isTransitioning = false;
            Debug.Log($"[StateMachine] Completed transition to: {transition.ToState.Name}");
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
                _stateTime = 0f;
                _currentState?.Enter();
                Debug.Log($"[StateMachine] Reverted to previous state: {_currentState?.GetType().Name}");
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

        /// <summary>
        /// 清除状态历史记录
        /// </summary>
        public void ClearHistory()
        {
            _stateHistory.Clear();
            Debug.Log("[StateMachine] State history cleared");
        }

        /// <summary>
        /// 检查当前状态是否为指定类型
        /// </summary>
        /// <typeparam name="T">要检查的状态类型</typeparam>
        /// <returns>如果当前状态为指定类型则返回true，否则返回false</returns>
        public bool IsInState<T>() where T : IState
        {
            return _currentState != null && _currentState.GetType() == typeof(T);
        }

        /// <summary>
        /// 获取当前状态的持续时间
        /// </summary>
        /// <returns>当前状态的持续时间</returns>
        public float GetTimeInCurrentState()
        {
            return _stateTime;
        }

        /// <summary>
        /// 获取状态历史记录
        /// </summary>
        /// <returns>状态历史记录</returns>
        public IReadOnlyList<System.Type> GetStateHistory()
        {
            return _stateHistory.Select(state => state.GetType()).ToArray();
        }
    }
}
