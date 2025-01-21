using UnityEngine;
using SkillModule.Events;
using SkillModule.Types;
using System.Collections.Generic;

namespace SkillModule.Effects
{
    public class AreaEffect : BaseEffect
    {
        private GameObject areaInstance;
        private Vector2 size;
        private float tickRate;
        private float lastTickTime;
        private LayerMask targetMask;
        private AreaEffectConfig config;
        private List<GameObject> affectedTargets;

        public AreaEffect(GameObject areaPrefab, Vector3 position, Vector2 size, float duration, AreaEffectConfig config) 
            : base(duration)
        {
            this.size = size;
            this.config = config;
            this.tickRate = config.TickRate;
            this.targetMask = config.TargetMask;
            this.affectedTargets = new List<GameObject>();

            if (areaPrefab != null)
            {
                areaInstance = GameObject.Instantiate(areaPrefab, position, Quaternion.identity);
                areaInstance.transform.localScale = new Vector3(size.x, size.y, 1);
            }
        }

        protected override void OnEffectStart()
        {
            if (areaInstance != null)
            {
                // 设置区域效果的视觉表现
                var renderer = areaInstance.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.color = config.AreaColor;
                }

                // 添加触发器碰撞体
                var collider = areaInstance.GetComponent<BoxCollider2D>();
                if (collider == null) collider = areaInstance.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
                collider.size = size;
            }
        }

        protected override void OnEffectUpdate()
        {
            if (Time.time - lastTickTime >= tickRate)
            {
                lastTickTime = Time.time;
                CheckAreaEffects();
            }

            if (areaInstance != null)
            {
                // 更新视觉效果
                UpdateVisuals();
            }
        }

        private void CheckAreaEffects()
        {
            // 检测区域内的目标
            var hits = Physics2D.OverlapBoxAll(
                areaInstance.transform.position,
                size,
                0,
                targetMask
            );

            foreach (var hit in hits)
            {
                if (!affectedTargets.Contains(hit.gameObject))
                {
                    // 新进入区域的目标
                    OnTargetEnterArea(hit.gameObject);
                }

                // 对区域内的目标施加效果
                ApplyAreaEffect(hit.gameObject);
            }

            // 检查已经离开区域的目标
            affectedTargets.RemoveAll(target =>
            {
                if (target == null) return true;
                
                bool stillInArea = false;
                foreach (var hit in hits)
                {
                    if (hit.gameObject == target)
                    {
                        stillInArea = true;
                        break;
                    }
                }

                if (!stillInArea)
                {
                    OnTargetExitArea(target);
                }

                return !stillInArea;
            });
        }

        private void ApplyAreaEffect(GameObject target)
        {
            eventManager.TriggerEvent(new SkillEffectEvent
            {
                EffectId = effectId,
                Type = config.EffectType,
                Target = target,
                Parameters = config.EffectParameters
            });
        }

        private void OnTargetEnterArea(GameObject target)
        {
            affectedTargets.Add(target);
        }

        private void OnTargetExitArea(GameObject target)
        {
            // 可以在这里处理目标离开区域的逻辑
        }

        private void UpdateVisuals()
        {
            float normalizedTime = elapsedTime / duration;
            
            // 更新颜色和透明度
            var renderer = areaInstance.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Color color = config.AreaColor;
                color.a = Mathf.Lerp(config.StartAlpha, config.EndAlpha, normalizedTime);
                renderer.color = color;
            }

            // 可以添加其他视觉效果（如脉冲、粒子等）
        }

        protected override void OnEffectEnd()
        {
            if (areaInstance != null)
            {
                GameObject.Destroy(areaInstance);
            }
        }

        protected override void OnEffectInterrupt(string reason)
        {
            OnEffectEnd();
        }
    }

    public struct AreaEffectConfig
    {
        public GameObject SkillPrefab;
        public Color AreaColor;
        public float StartAlpha;
        public float EndAlpha;
        public float TickRate;
        public LayerMask TargetMask;
        public SkillEffectType EffectType;
        public float[] EffectParameters;
    }
} 