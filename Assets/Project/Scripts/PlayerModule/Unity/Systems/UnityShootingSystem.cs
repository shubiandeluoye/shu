using UnityEngine;
using PlayerModule.Core.Systems;
using PlayerModule.Data;
using UnityEngine.InputSystem;

namespace PlayerModule.Unity.Systems
{
    public class UnityShootingSystem : MonoBehaviour
    {
        private ShootingSystem coreSystem;
        private Transform shootPoint;
        
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private ParticleSystem shootEffect;
        [SerializeField] private AudioSource shootAudio;

        public void Initialize(ShootingSystem system, Transform shootTransform)
        {
            coreSystem = system;
            shootPoint = shootTransform;
            
            // 订阅事件
            // 处理表现效果
        }

        public void OnStraightShoot(InputAction.CallbackContext context)
        {
            if (!context.performed || coreSystem == null) return;
            coreSystem.Shoot(shootPoint.position, Vector3.right);
        }

        public void OnAngleShoot(InputAction.CallbackContext context, float angle)
        {
            if (!context.performed || coreSystem == null) return;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
            coreSystem.Shoot(shootPoint.position, direction);
        }

        private void OnBulletTypeChanged(BulletType newType)
        {
            // 更新子弹预制体和特效
            UpdateBulletVisuals(newType);
        }

        private void UpdateBulletVisuals(BulletType type)
        {
            // 根据子弹类型更新视觉效果
        }

        private void PlayShootEffects()
        {
            if (shootEffect != null)
            {
                shootEffect.Play();
            }

            if (shootAudio != null)
            {
                shootAudio.Play();
            }
        }
    }
} 