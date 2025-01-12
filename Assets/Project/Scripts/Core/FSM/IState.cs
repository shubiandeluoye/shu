using UnityEngine;

namespace Core.FSM
{
    /// <summary>
    /// 状态实现的接口定义
    /// 定义了状态机中每个状态必须实现的基本方法
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// 进入状态时调用
        /// </summary>
        void Enter();

        /// <summary>
        /// 状态更新时调用
        /// </summary>
        void Update();

        /// <summary>
        /// 退出状态时调用
        /// </summary>
        void Exit();
    }
}
