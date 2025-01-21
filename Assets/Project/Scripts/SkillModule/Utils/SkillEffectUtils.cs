using UnityEngine;

namespace SkillModule.Utils
{
    public static class SkillEffectUtils
    {
        public static GameObject SpawnEffect(GameObject effectPrefab, Vector3 position, float duration)
        {
            if (effectPrefab == null) return null;

            var effect = GameObject.Instantiate(effectPrefab, position, Quaternion.identity);
            if (duration > 0)
            {
                GameObject.Destroy(effect, duration);
            }
            return effect;
        }

        public static void SetEffectColor(GameObject effect, Color color)
        {
            if (effect == null) return;

            var renderer = SkillUtils.SafeGetComponent<SpriteRenderer>(effect);
            if (renderer != null)
            {
                renderer.color = color;
            }

            var particleSystem = SkillUtils.SafeGetComponent<ParticleSystem>(effect);
            if (particleSystem != null)
            {
                var main = particleSystem.main;
                main.startColor = color;
            }
        }
    }
} 