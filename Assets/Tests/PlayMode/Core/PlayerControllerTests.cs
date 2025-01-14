using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Gameplay;
using Gameplay.Core;
using Core.EventSystem;

public class PlayerControllerTests
{
    private GameObject playerObject;
    private PlayerController playerController;
    private EventManager eventManager;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // 创建 EventManager
        GameObject eventManagerObject = new GameObject("EventManager");
        eventManager = eventManagerObject.AddComponent<EventManager>();

        // 创建 PlayerController
        playerObject = new GameObject("Player");
        playerController = playerObject.AddComponent<PlayerController>();
        yield return null;
    }

    [UnityTest]
    public IEnumerator PlayerController_Movement_StaysWithinBounds()
    {
        // 测试向右移动到边界
        Vector2 rightMovement = Vector2.right;
        for (int i = 0; i < 100; i++)
        {
            playerController.SimulateMove(rightMovement);
            yield return null;
        }

        Assert.That(playerObject.transform.position.x, Is.LessThanOrEqualTo(GameplayConstants.Bounds.HALF_WIDTH));
    }

    [UnityTest]
    public IEnumerator PlayerController_TakeDamage_TriggersEvent()
    {
        bool eventTriggered = false;
        float testDamage = 10f;

        // 监听伤害事件
        EventManager.Instance.AddListener<GameplayEvents.PlayerDamagedEvent>(evt => 
        {
            eventTriggered = true;
            Assert.That(evt.Damage, Is.EqualTo(testDamage));
        });

        playerController.TakeDamage(testDamage);
        yield return null;

        Assert.That(eventTriggered, Is.True, "伤害事件应该被触发");
    }

    [UnityTest]
    public IEnumerator PlayerController_Death_TriggersGameEnd()
    {
        bool gameEndTriggered = false;

        // 监听游戏结束事件
        EventManager.Instance.AddListener<GameplayEvents.GameEndEvent>(evt => 
        {
            gameEndTriggered = true;
            Assert.That(evt.LoserId, Is.EqualTo(playerObject.GetInstanceID()));
        });

        // 造成致命伤害
        playerController.TakeDamage(1000f);
        yield return null;

        Assert.That(gameEndTriggered, Is.True, "游戏结束事件应该被触发");
        Assert.That(playerObject.activeInHierarchy, Is.False, "玩家对象应该被禁用");
    }

    [UnityTearDown]
    public IEnumerator Cleanup()
    {
        if (playerObject != null)
            Object.Destroy(playerObject);
        if (eventManager != null)
            Object.Destroy(eventManager.gameObject);
        yield return null;
    }
} 