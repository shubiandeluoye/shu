namespace GameModule.Core.Network
{
    public interface INetworkAuthority
    {
        bool HasStateAuthority { get; }
        int LocalPlayerId { get; }
        void SendMessage<T>(T message) where T : struct;
        void BroadcastEvent<T>(T eventData) where T : struct;
    }
} 