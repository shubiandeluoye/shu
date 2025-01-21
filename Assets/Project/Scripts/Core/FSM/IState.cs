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

        /// <summary>
        /// 检查是否可以进入该状态
        /// </summary>
        bool CanEnter();

        /// <summary>
        /// 检查是否可以退出该状态
        /// </summary>
        bool CanExit();

        /// <summary>
        /// 状态被暂停时调用
        /// </summary>
        void OnSuspend();

        /// <summary>
        /// 状态被恢复时调用
        /// </summary>
        void OnResume();

        /// <summary>
        /// 获取状态名称
        /// </summary>
        string GetStateName();

        /// <summary>
        /// 获取状态当前状态
        /// </summary>
        StateStatus GetStatus();
    }

    /// <summary>
    /// 状态信息枚举
    /// </summary>
    public enum StateStatus
    {
        None,
        Entering,
        Active,
        Suspended,
        Exiting
    }
}
