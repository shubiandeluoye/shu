using Fusion;  // 临时使用 Fusion

namespace Core.Network
{
    /// <summary>
    /// 网络权限接口
    /// TODO: [网络重构] 当前直接使用 Fusion，后期需要重构为自定义网络抽象层
    /// </summary>
    public interface INetworkAuthority
    {
        /// <summary>
        /// 检查是否有状态更改权限
        /// TODO: [网络重构] Fusion 特有的概念，需要重新设计
        /// </summary>
        bool HasStateAuthority();

        /// <summary>
        /// 检查是否有输入权限
        /// TODO: [网络重构] Fusion 特有的概念，需要重新设计
        /// </summary>
        bool HasInputAuthority();

        /// <summary>
        /// 检查玩家引用是否有效
        /// TODO: [网络重构] 当前使用 Fusion.PlayerRef，需要替换为自定义类型
        /// </summary>
        bool IsPlayerValid(PlayerRef playerRef);

        // 可以根据需要添加其他网络相关的接口
    }
} 