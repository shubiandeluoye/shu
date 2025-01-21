using UnityEngine;

namespace SkillModule.Utils
{
    public static class SkillUtils
    {
        public static Sprite CreateDefaultSprite()
        {
            var texture = new Texture2D(32, 32);
            var pixels = new Color[32 * 32];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        }

        public static void SafeDestroy(GameObject obj)
        {
            if (obj != null)
            {
                GameObject.Destroy(obj);
            }
        }

        public static T SafeGetComponent<T>(GameObject obj) where T : Component
        {
            if (obj == null) return null;
            return obj.GetComponent<T>();
        }
    }
} 