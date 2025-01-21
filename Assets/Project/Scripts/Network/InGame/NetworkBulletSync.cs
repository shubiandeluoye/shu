using UnityEngine;
using Fusion;

namespace Core.Network
{
    /// <summary>
    /// 子弹同步组件
    /// 负责：
    /// 1. 子弹生成和销毁
    /// 2. 子弹轨迹同步
    /// 3. 子弹碰撞检测
    /// 4. 子弹反弹计数
    /// </summary>
    public class NetworkBulletSync : NetworkBehaviour
    {
        [Networked] private NetworkDictionary<int, BulletData> ActiveBullets { get; }
        [Networked] private int NextBulletId { get; set; }

        public struct BulletData : INetworkStruct
        {
            public Vector3 Position;
            public Vector3 Direction;
            public float Speed;
            public int BounceCount;
            public NetworkBool IsActive;
        }

        public void SyncBullet(Vector3 position, Vector3 direction, float speed)
        {
            if (!Runner.IsServer) return;

            var bulletId = NextBulletId++;
            var bulletData = new BulletData
            {
                Position = position,
                Direction = direction,
                Speed = speed,
                BounceCount = 0,
                IsActive = true
            };

            ActiveBullets.Set(bulletId, bulletData);
            RPC_OnBulletSpawned(bulletId, bulletData);
        }

        public void UpdateBulletPosition(int bulletId, Vector3 newPosition, int bounceCount)
        {
            if (!Runner.IsServer) return;

            if (ActiveBullets.TryGet(bulletId, out var bulletData))
            {
                bulletData.Position = newPosition;
                bulletData.BounceCount = bounceCount;
                ActiveBullets.Set(bulletId, bulletData);
                RPC_OnBulletUpdated(bulletId, newPosition, bounceCount);
            }
        }

        public void DeactivateBullet(int bulletId)
        {
            if (!Runner.IsServer) return;

            if (ActiveBullets.TryGet(bulletId, out var bulletData))
            {
                bulletData.IsActive = false;
                ActiveBullets.Set(bulletId, bulletData);
                RPC_OnBulletDeactivated(bulletId);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnBulletSpawned(int bulletId, BulletData data)
        {
            if (ActiveBullets.TryGet(bulletId, out _)) return;
            ActiveBullets.Set(bulletId, data);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnBulletUpdated(int bulletId, Vector3 position, int bounceCount)
        {
            if (ActiveBullets.TryGet(bulletId, out var bulletData))
            {
                bulletData.Position = position;
                bulletData.BounceCount = bounceCount;
                ActiveBullets.Set(bulletId, bulletData);
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnBulletDeactivated(int bulletId)
        {
            if (ActiveBullets.TryGet(bulletId, out var bulletData))
            {
                bulletData.IsActive = false;
                ActiveBullets.Set(bulletId, bulletData);
            }
        }

        public BulletData? GetBulletData(int bulletId)
        {
            return ActiveBullets.TryGet(bulletId, out var data) ? data : null;
        }

        public void ClearAllBullets()
        {
            if (!Runner.IsServer) return;
            ActiveBullets.Clear();
            RPC_OnBulletsCleared();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_OnBulletsCleared()
        {
            ActiveBullets.Clear();
        }
    }
} 