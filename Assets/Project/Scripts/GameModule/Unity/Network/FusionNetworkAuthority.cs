using Fusion;
using GameModule.Core.Network;

namespace GameModule.Unity.Network
{
    public class FusionNetworkAuthority : INetworkAuthority
    {
        private readonly NetworkRunner runner;
        private readonly NetworkObject networkObject;

        public bool HasStateAuthority => networkObject.HasStateAuthority;
        public int LocalPlayerId => (int)runner.LocalPlayer;

        public FusionNetworkAuthority(NetworkRunner runner, NetworkObject networkObject)
        {
            this.runner = runner;
            this.networkObject = networkObject;
        }

        public void SendMessage<T>(T message) where T : struct
        {
            if (runner != null)
            {
                runner.Rpc(nameof(RpcMessage), message);
            }
        }

        public void BroadcastEvent<T>(T eventData) where T : struct
        {
            if (runner != null)
            {
                runner.Rpc(nameof(RpcBroadcast), eventData);
            }
        }

        [Rpc]
        private void RpcMessage<T>(T message) where T : struct
        {
            // 处理RPC消息
        }

        [Rpc]
        private void RpcBroadcast<T>(T eventData) where T : struct
        {
            // 广播事件
        }
    }
} 