using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Gameplay.Bullet;

public class BulletPrefabSetupTests
{
    private GameObject bulletPrefab;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        bulletPrefab = new GameObject("BulletPrefab");
        bulletPrefab.AddComponent<BulletPrefabSetup>();
        yield return null;
    }

    [Test]
    public void BulletPrefab_HasRequiredComponents()
    {
        Assert.That(bulletPrefab.GetComponent<SpriteRenderer>(), Is.Not.Null, "应该有 SpriteRenderer");
        Assert.That(bulletPrefab.GetComponent<CircleCollider2D>(), Is.Not.Null, "应该有 CircleCollider2D");
        Assert.That(bulletPrefab.GetComponent<Rigidbody2D>(), Is.Not.Null, "应该有 Rigidbody2D");
        Assert.That(bulletPrefab.GetComponent<BulletController>(), Is.Not.Null, "应该有 BulletController");
    }

    [Test]
    public void BulletPrefab_HasCorrectConfiguration()
    {
        var collider = bulletPrefab.GetComponent<CircleCollider2D>();
        Assert.That(collider.radius, Is.EqualTo(0.5f), "碰撞器半径应该是 0.5");
        Assert.That(collider.isTrigger, Is.True, "碰撞器应该是触发器");

        var rigidbody = bulletPrefab.GetComponent<Rigidbody2D>();
        Assert.That(rigidbody.isKinematic, Is.True, "刚体应该是运动学的");

        Assert.That(bulletPrefab.layer, Is.EqualTo(LayerMask.NameToLayer("Bullet")), "应该在 Bullet 层");
        Assert.That(bulletPrefab.tag, Is.EqualTo("Bullet"), "应该有 Bullet 标签");
    }

    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        if (bulletPrefab != null)
            Object.Destroy(bulletPrefab);
        yield return null;
    }
} 