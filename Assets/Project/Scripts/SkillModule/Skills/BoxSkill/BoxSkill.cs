using UnityEngine;
using Core.EventSystem;
using SkillModule.Utils;
using SkillModule.Core;

namespace SkillModule.Skills
{
    public class BoxSkill : BaseSkill
    {
        private BoxConfig boxConfig;

        public BoxSkill(BoxConfig config) : base(config)
        {
            boxConfig = config;
        }

        public override bool Execute(Vector3 position, Vector3 direction)
        {
            if (!isReady) return false;

            Vector3 placePosition = position + direction.normalized * boxConfig.PlaceDistance;
            
            // 检查放置位置
            if (boxConfig.CheckGroundCollision)
            {
                var hit = Physics2D.OverlapBox(
                    placePosition, 
                    Vector2.one * boxConfig.BoxSize, 
                    0f, 
                    boxConfig.PlacementMask
                );
                
                if (hit != null) return false;
            }

            // 生成盒子
            SpawnBox(placePosition);

            // 播放创建音效
            SkillAudioUtils.PlaySkillSound(
                boxConfig.CreateSound,
                placePosition,
                config.SoundVolume
            );

            // 触发技能事件
            TriggerSkillStartEvent(placePosition, direction);

            // 开始冷却
            StartCooldown();

            return true;
        }

        private void SpawnBox(Vector3 position)
        {
            GameObject box;
            if (boxConfig.SkillPrefab != null)
            {
                box = GameObject.Instantiate(boxConfig.SkillPrefab, position, Quaternion.identity);
            }
            else
            {
                box = new GameObject("Box");
                box.transform.position = position;
            }

            // 设置大小
            box.transform.localScale = Vector3.one * boxConfig.BoxSize;

            // 添加刚体
            var rigidbody = SkillUtils.SafeGetComponent<Rigidbody2D>(box);
            if (rigidbody == null) rigidbody = box.AddComponent<Rigidbody2D>();
            rigidbody.mass = boxConfig.Mass;
            rigidbody.drag = boxConfig.Drag;
            rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

            // 设置碰撞体
            var collider = SkillUtils.SafeGetComponent<BoxCollider2D>(box);
            if (collider == null) collider = box.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
            if (boxConfig.UsePhysicsMaterial && boxConfig.BoxMaterial != null)
            {
                collider.sharedMaterial = boxConfig.BoxMaterial;
            }

            // 设置渲染器
            var renderer = SkillUtils.SafeGetComponent<SpriteRenderer>(box);
            if (renderer == null) renderer = box.AddComponent<SpriteRenderer>();
            renderer.sprite = SkillUtils.CreateDefaultSprite();
            renderer.color = boxConfig.BoxColor;

            // 设置音效
            SkillAudioUtils.CreateAudioSource(
                box,
                boxConfig.ImpactSound,
                config.SoundVolume
            );

            // 设置盒子属性
            var boxBehaviour = SkillUtils.SafeGetComponent<BoxBehaviour>(box);
            if (boxBehaviour == null) boxBehaviour = box.AddComponent<BoxBehaviour>();
            boxBehaviour.Initialize(
                boxConfig.BoxDurability, 
                boxConfig.CanBePushed, 
                boxConfig.PushForce
            );

            // 生成创建特效
            if (boxConfig.CreateEffectPrefab != null)
            {
                var effect = SkillEffectUtils.SpawnEffect(
                    boxConfig.CreateEffectPrefab,
                    position,
                    boxConfig.CreateEffectDuration
                );
                SkillEffectUtils.SetEffectColor(effect, boxConfig.BoxColor);
            }
        }
    }
} 