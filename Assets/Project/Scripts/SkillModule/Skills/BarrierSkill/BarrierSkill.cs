using UnityEngine;
using SkillModule.Core;
using Core.EventSystem;
using SkillModule.Utils;
using SkillModule.Effects;

namespace SkillModule.Skills
{
    public class BarrierSkill : BaseSkill
    {
        private BarrierConfig barrierConfig;

        public BarrierSkill(BarrierConfig config) : base(config)
        {
            barrierConfig = config;
        }

        public override bool Execute(Vector3 position, Vector3 direction)
        {
            if (!isReady) return false;

            // 生成屏障
            SpawnBarrier(position, direction);

            // 播放创建音效
            SkillAudioUtils.PlaySkillSound(
                barrierConfig.CreateSound, 
                position, 
                barrierConfig.SoundVolume
            );

            // 触发技能事件
            TriggerSkillStartEvent(position, direction);

            // 开始冷却
            StartCooldown();

            return true;
        }

        private void SpawnBarrier(Vector3 position, Vector3 direction)
        {
            GameObject barrier;
            if (barrierConfig.SkillPrefab != null)
            {
                barrier = GameObject.Instantiate(barrierConfig.SkillPrefab, position, Quaternion.identity);
            }
            else
            {
                barrier = new GameObject("Barrier");
                barrier.transform.position = position;
            }

            // 设置大小和旋转
            barrier.transform.localScale = new Vector3(
                barrierConfig.BarrierWidth,
                barrierConfig.BarrierHeight,
                1f
            );

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            barrier.transform.rotation = Quaternion.Euler(0, 0, angle);

            // 设置渲染器
            var renderer = SkillUtils.SafeGetComponent<SpriteRenderer>(barrier);
            if (renderer == null)
            {
                renderer = barrier.AddComponent<SpriteRenderer>();
                renderer.sprite = SkillUtils.CreateDefaultSprite();
            }
            renderer.color = barrierConfig.BarrierColor;

            // 设置碰撞体
            var collider = SkillUtils.SafeGetComponent<BoxCollider2D>(barrier);
            if (collider == null)
            {
                collider = barrier.AddComponent<BoxCollider2D>();
            }

            // 设置音效
            SkillAudioUtils.CreateAudioSource(
                barrier, 
                barrierConfig.BlockSound, 
                barrierConfig.SoundVolume
            );

            // 添加行为脚本
            var barrierBehaviour = SkillUtils.SafeGetComponent<BarrierBehaviour>(barrier);
            if (barrierBehaviour == null)
            {
                barrierBehaviour = barrier.AddComponent<BarrierBehaviour>();
            }

            // 初始化屏障
            barrierBehaviour.Initialize(
                barrierConfig.BarrierHealth,
                barrierConfig.BlocksProjectiles,
                barrierConfig.BlocksPlayers,
                barrierConfig.DamageReduction,
                config.Duration
            );

            // 创建屏障效果
            var barrierEffect = new BarrierEffect(
                barrier,  // 使用已创建的barrier对象
                position,
                config.Duration,
                new BarrierEffectConfig
                {
                    Width = barrierConfig.BarrierWidth,
                    Height = barrierConfig.BarrierHeight,
                    Health = barrierConfig.BarrierHealth,
                    BarrierColor = barrierConfig.BarrierColor,
                    DamagedColor = barrierConfig.DamagedColor,
                    PulseEffect = barrierConfig.PulseEffect,
                    PulseSpeed = barrierConfig.PulseSpeed,
                    PulseIntensity = barrierConfig.PulseIntensity
                }
            );

            // 生成创建特效
            if (barrierConfig.ImpactEffectPrefab != null)
            {
                var effect = SkillEffectUtils.SpawnEffect(
                    barrierConfig.ImpactEffectPrefab,
                    position,
                    barrierConfig.ImpactEffectDuration
                );
                SkillEffectUtils.SetEffectColor(effect, barrierConfig.BarrierColor);
            }
        }
    }
} 