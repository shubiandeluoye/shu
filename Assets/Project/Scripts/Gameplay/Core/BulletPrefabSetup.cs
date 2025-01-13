using UnityEngine;

/// <summary>
/// Sets up the bullet prefab with required components and properties
/// </summary>
public class BulletPrefabSetup : MonoBehaviour
{
    private void Awake()
    {
        // Add required components
        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        var circleCollider = gameObject.AddComponent<CircleCollider2D>();
        var rigidbody = gameObject.AddComponent<Rigidbody2D>();
        var bulletController = gameObject.AddComponent<BulletController>();

        // Configure sprite renderer
        spriteRenderer.color = new Color(1f, 0.5f, 0f); // Orange
        spriteRenderer.sprite = CreateCircleSprite();

        // Configure collider
        circleCollider.radius = 0.5f;
        circleCollider.isTrigger = true;

        // Configure rigidbody
        rigidbody.isKinematic = true;
        rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        rigidbody.collisionDetection = CollisionDetectionMode2D.Continuous;

        // Set layer and tag
        gameObject.layer = LayerMask.NameToLayer("Projectile");
        gameObject.tag = "Bullet";
    }

    private Sprite CreateCircleSprite()
    {
        // Create a simple circle sprite
        var texture = new Texture2D(32, 32);
        var center = new Vector2(16, 16);
        var radius = 14f;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                var distance = Vector2.Distance(new Vector2(x, y), center);
                var alpha = distance <= radius ? 1f : 0f;
                texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
