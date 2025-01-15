using UnityEngine;

namespace Core.Testing
{
    public interface ITestablePlayer
    {
        // 属性
        Vector3 Position { get; }
        float Health { get; }
        bool IsAlive { get; }

        // 测试方法
        void SimulateMove(Vector2 direction);
        void SimulateShoot(float angle);
        // 暂时注释掉这两个方法，因为现在不需要
        // BulletType GetCurrentBulletType();
        // int GetBounceCount();
    }
} 