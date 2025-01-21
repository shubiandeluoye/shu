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
        
        // 新增的接口方法
        bool CanBeSpawned(); // 检查对象是否可以被生成
        bool CanBeRecycled(); // 检查对象是否可以被回收
        void OnPoolCreate(); // 对象被创建到池中时调用
        void OnPoolDestroy(); // 对象被从池中销毁时调用
        void Reset(); // 重置对象状态
        
        // 获取对象信息
        PoolableStatus GetStatus(); // 获取对象当前状态
        float GetLifetime(); // 获取对象生命周期
        int GetUseCount(); // 获取使用次数
    }

    // 可池化对象状态枚举
    public enum PoolableStatus
    {
        None,
        Created,
        Spawned,
        Active,
        Recycled,
        Destroyed
    }

    // 池化对象配置接口
    public interface IPoolableConfig
    {
        float LifetimeLimit { get; set; } // 生命周期限制
        int MaxUseCount { get; set; } // 最大使用次数
        bool AutoRecycle { get; set; } // 是否自动回收
        float RecycleDelay { get; set; } // 回收延迟
    }
} 