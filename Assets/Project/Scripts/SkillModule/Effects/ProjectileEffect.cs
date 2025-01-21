using UnityEngine;
using SkillModule.Events;
using SkillModule.Types;

namespace SkillModule.Effects
{
    /// <summary>
    /// 投射物效果
    /// 处理子弹、飞行道具等效果
    /// </summary>
    public class ProjectileEffect : BaseEffect
    {
        private GameObject projectile;
        private Vector3 direction;
        private float speed;
        private float damage;
        private bool hasHit;

        public ProjectileEffect(GameObject projectilePrefab, Vector3 direction, float speed, float damage, float duration) 
            : base(duration)
        {
            this.projectile = projectilePrefab;
            this.direction = direction.normalized;
            this.speed = speed;
            this.damage = damage;
        }

        protected override void OnEffectStart()
        {
            if (projectile != null)
            {
                // 设置子弹的初始属性
                var rigidbody = projectile.GetComponent<Rigidbody2D>();
                if (rigidbody != null)
                {
                    rigidbody.velocity = direction * speed;
                }

                // 添加碰撞检测
                var collider = projectile.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.isTrigger = true;
                }
            }
        }

        protected override void OnEffectUpdate()
        {
            if (projectile != null && !hasHit)
            {
                // 检测碰撞
                var hit = Physics2D.Raycast(
                    projectile.transform.position,
                    direction,
                    speed * Time.deltaTime
                );

                if (hit.collider != null)
                {
                    OnProjectileHit(hit);
                }
            }
        }

        private void OnProjectileHit(RaycastHit2D hit)
        {
            hasHit = true;

            // 触发伤害事件
            eventManager.TriggerEvent(new SkillEffectEvent
            {
                EffectId = effectId,
                Type = (SkillEffectType)EffectType.Damage,
                Target = hit.collider.gameObject,
                Parameters = new float[] { damage }
            });

            // 可以在这里添加击中特效
            End();
        }

        protected override void OnEffectEnd()
        {
            if (projectile != null)
            {
                GameObject.Destroy(projectile);
            }
        }

        protected override void OnEffectInterrupt(string reason)
        {
            OnEffectEnd();
        }
    }
} 