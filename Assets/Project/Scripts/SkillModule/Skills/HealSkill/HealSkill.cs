using UnityEngine;
using Core.EventSystem;
using SkillModule.Utils;
using SkillModule.Core;
using SkillModule.Events;
using SkillModule.Effects;

namespace SkillModule.Skills
{
    /// <summary>
    /// 回血技能
    /// 功能：
    /// 1. 恢复指定数值的生命值
    /// 2. 播放回血特效
    /// 3. 有冷却时间
    /// </summary>
    public class HealSkill : BaseSkill
    {
        private HealConfig healConfig;
        private GameObject owner;

        public HealSkill(HealConfig config, GameObject owner) : base(config)
        {
            healConfig = config;
            this.owner = owner;
        }

        public override bool Execute(Vector3 position, Vector3 direction)
        {
            if (!isReady) return false;

            bool success = false;

            // 处理范围治疗
            if (healConfig.HealRadius > 0)
            {
                success = ExecuteAreaHeal(position);
            }
            else
            {
                success = ExecuteSingleHeal(position);
            }

            // 播放音效
            SkillAudioUtils.PlaySkillSound(
                success ? healConfig.HealSound : healConfig.FailSound,
                position,
                healConfig.SoundVolume
            );

            // 生成治疗特效
            if (success && healConfig.UseParticles)
            {
                SpawnHealEffect(position);
            }

            // 触发技能事件
            TriggerSkillStartEvent(position, direction);

            // 开始冷却
            StartCooldown();

            return success;
        }

        private bool ExecuteAreaHeal(Vector3 position)
        {
            bool anyTargetHealed = false;
            var colliders = Physics2D.OverlapCircleAll(position, healConfig.HealRadius, healConfig.TargetLayers);
            
            foreach (var collider in colliders)
            {
                if (ApplyHeal(collider.gameObject))
                {
                    anyTargetHealed = true;
                    
                    // 为每个治疗目标生成特效
                    if (healConfig.UseParticles)
                    {
                        SpawnHealEffect(collider.transform.position);
                    }
                }
            }

            return anyTargetHealed;
        }

        private bool ExecuteSingleHeal(Vector3 position)
        {
            var hit = Physics2D.Raycast(position, Vector2.zero, 0f, healConfig.TargetLayers);
            if (hit.collider != null)
            {
                return ApplyHeal(hit.collider.gameObject);
            }
            return false;
        }

        private bool ApplyHeal(GameObject target)
        {
            // 发送治疗事件
            eventManager.TriggerEvent(new HealEvent
            {
                Source = owner,
                Target = target,
                Amount = healConfig.HealAmount,
                IsPercentage = healConfig.IsPercentage,
                CanOverheal = healConfig.CanOverheal
            });

            // 应用Buff效果
            if (healConfig.ApplyBuff)
            {
                ApplyHealBuff(target);
            }

            return true;
        }

        private void ApplyHealBuff(GameObject target)
        {
            if (healConfig.BuffEffectPrefab != null)
            {
                var buffEffect = SkillEffectUtils.SpawnEffect(
                    healConfig.BuffEffectPrefab,
                    target.transform.position,
                    healConfig.BuffDuration
                );
                
                if (buffEffect != null)
                {
                    buffEffect.transform.SetParent(target.transform);
                    SkillEffectUtils.SetEffectColor(buffEffect, healConfig.HealColor);
                }
            }

            // 创建治疗效果
            var healEffect = new HealEffect(
                healConfig.BuffEffectPrefab,
                target.transform.position,
                healConfig.HealAmount,
                healConfig.BuffDuration
            );

            // 初始化效果配置
            healEffect.Initialize(new HealEffectConfig
            {
                Color = healConfig.HealColor,
                Scale = healConfig.EffectScale,
                Duration = healConfig.BuffDuration
            });

            // 通过事件系统应用效果
            eventManager.TriggerEvent(new EffectEvent
            {
                Target = target,
                EffectId = healConfig.SkillId,
                Duration = healConfig.BuffDuration,
                Strength = healConfig.BuffStrength
            });
        }

        private void SpawnHealEffect(Vector3 position)
        {
            // 创建粒子效果
            var effect = new GameObject("HealEffect");
            effect.transform.position = position;

            var particleSystem = effect.AddComponent<ParticleSystem>();
            var main = particleSystem.main;
            main.startColor = healConfig.HealColor;
            main.startSize = healConfig.EffectScale;
            main.startLifetime = healConfig.ParticleLifetime;
            main.maxParticles = healConfig.ParticleCount;

            var emission = particleSystem.emission;
            emission.rateOverTime = healConfig.ParticleCount / healConfig.ParticleLifetime;

            var velocityOverLifetime = particleSystem.velocityOverLifetime;
            velocityOverLifetime.radial = healConfig.ParticleSpeed;

            // 自动销毁
            GameObject.Destroy(effect, healConfig.ParticleLifetime);
        }
    }
} 