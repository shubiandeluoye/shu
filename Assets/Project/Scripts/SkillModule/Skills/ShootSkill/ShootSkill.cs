using UnityEngine;
using Core.EventSystem;
using SkillModule.Utils;
using SkillModule.Core;

namespace SkillModule.Skills
{
    public class ShootSkill : BaseSkill
    {
        private ShootConfig shootConfig;

        public ShootSkill(ShootConfig config) : base(config)
        {
            shootConfig = config;
        }

        public override bool Execute(Vector3 position, Vector3 direction)
        {
            if (!isReady) return false;

            // 生成多个方向的子弹
            foreach (float angle in shootConfig.ShootAngles)
            {
                Vector3 bulletDirection = RotateVector(direction, angle);
                SpawnBullet(position, bulletDirection);
            }

            // 播放发射音效
            SkillAudioUtils.PlaySkillSound(
                shootConfig.ShootSound,
                position,
                shootConfig.SoundVolume
            );

            // 生成发射特效
            if (shootConfig.ShootEffectPrefab != null)
            {
                var effect = SkillEffectUtils.SpawnEffect(
                    shootConfig.ShootEffectPrefab,
                    position,
                    shootConfig.ShootEffectDuration
                );
                SkillEffectUtils.SetEffectColor(effect, shootConfig.ShootEffectColor);
            }

            // 触发技能事件
            TriggerSkillStartEvent(position, direction);

            // 开始冷却
            StartCooldown();

            return true;
        }

        private void SpawnBullet(Vector3 position, Vector3 direction)
        {
            GameObject bullet = null;
            if (shootConfig.SkillPrefab != null)
            {
                bullet = GameObject.Instantiate(shootConfig.SkillPrefab, position, Quaternion.identity);
            }
            else
            {
                bullet = new GameObject("Bullet");
                bullet.transform.position = position;
            }

            // 设置渲染器
            var renderer = SkillUtils.SafeGetComponent<SpriteRenderer>(bullet);
            if (renderer == null)
            {
                renderer = bullet.AddComponent<SpriteRenderer>();
                renderer.sprite = SkillUtils.CreateDefaultSprite();
            }
            renderer.color = shootConfig.BulletColor;

            // 设置碰撞体
            var collider = SkillUtils.SafeGetComponent<CircleCollider2D>(bullet);
            if (collider == null)
            {
                collider = bullet.AddComponent<CircleCollider2D>();
            }
            collider.radius = shootConfig.BulletRadius;
            collider.isTrigger = true;

            // 设置音效
            SkillAudioUtils.CreateAudioSource(
                bullet,
                shootConfig.BulletImpactSound,
                shootConfig.SoundVolume
            );

            // 设置子弹控制器
            var bulletController = SkillUtils.SafeGetComponent<BulletController>(bullet);
            if (bulletController == null)
            {
                bulletController = bullet.AddComponent<BulletController>();
            }

            // 初始化子弹
            bulletController.Initialize(
                direction * shootConfig.BulletSpeed,
                shootConfig.BulletDamage,
                shootConfig.BulletLifetime
            );
        }

        private Vector3 RotateVector(Vector3 vector, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);
            
            return new Vector3(
                vector.x * cos - vector.y * sin,
                vector.x * sin + vector.y * cos,
                0
            );
        }
    }
} 