using UnityEngine;
using Fusion;
using MapModule.Shapes;

namespace Core.Network
{
    public class NetworkMapSystemInitializedEvent
    {
        public bool IsInitialized { get; set; }
        public NetworkId SystemId { get; set; }  // 这个后期可能需要改
    }

    // 这个事件应该在 MapModule.Shapes 中定义，这里删除
    // public class ShapeStateChangedEvent
    // {
    //     public ShapeState State { get; set; }
    // }

    // 使用接口或抽象类来定义网络状态
    public interface INetworkState
    {
        int Id { get; }
        Vector3 Position { get; }
        Quaternion Rotation { get; }
        Vector3 Scale { get; }
    }

    public class NetworkShapeStateEvent
    {
        public INetworkState State { get; set; }  // 使用接口
        public int ShapeId { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
    }

    public class NetworkCentralAreaEvent
    {
        public Vector3 Position { get; set; }
        public float Radius { get; set; }
        public bool IsActive { get; set; }
    }
} 