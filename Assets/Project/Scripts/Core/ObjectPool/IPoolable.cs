using UnityEngine;

namespace Core.ObjectPool
{
    /// <summary>
    /// 可池化对象的接口
    /// </summary>
    public interface IPoolable
    {
        void OnSpawn();      // 从对象池取出时调用
        void OnRecycle();    // 回收到对象池时调用
    }
} 