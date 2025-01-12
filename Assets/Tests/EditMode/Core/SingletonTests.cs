using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

[TestFixture]
public class SingletonTests
{
    private class TestSingleton : Singleton<TestSingleton> { }

    [Test]
    public void Instance_WhenAccessed_CreatesNewInstanceIfNull()
    {
        // Arrange
        var gameObject = new GameObject();
        var singleton = gameObject.AddComponent<TestSingleton>();

        // Act
        var instance = TestSingleton.Instance;

        // Assert
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance, Is.EqualTo(singleton));
    }

    [Test]
    public void Instance_WhenAccessedMultipleTimes_ReturnsSameInstance()
    {
        // Arrange
        var gameObject = new GameObject();
        gameObject.AddComponent<TestSingleton>();

        // Act
        var instance1 = TestSingleton.Instance;
        var instance2 = TestSingleton.Instance;

        // Assert
        Assert.That(instance1, Is.EqualTo(instance2));
    }

    [UnityTest]
    public IEnumerator Instance_WhenAccessedFromMultipleThreads_MaintainsThreadSafety()
    {
        // Arrange
        var tasks = new Task[10];
        TestSingleton firstInstance = null;
        var lockObject = new object();

        // Act
        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                var instance = TestSingleton.Instance;
                lock (lockObject)
                {
                    if (firstInstance == null)
                        firstInstance = instance;
                }
            });
        }

        yield return new WaitUntil(() => Task.WhenAll(tasks).IsCompleted);

        // Assert
        foreach (var task in tasks)
        {
            Assert.That(task.IsCompleted, Is.True);
            Assert.That(task.IsFaulted, Is.False);
        }

        Assert.That(TestSingleton.Instance, Is.EqualTo(firstInstance));
    }

    [Test]
    public void Instance_WhenDuplicateExists_DestroysNewInstance()
    {
        // Arrange
        var gameObject1 = new GameObject();
        var singleton1 = gameObject1.AddComponent<TestSingleton>();
        var gameObject2 = new GameObject();
        
        // Act
        var singleton2 = gameObject2.AddComponent<TestSingleton>();
        
        // Assert
        Assert.That(singleton2 == null || !singleton2.gameObject.activeInHierarchy);
        Assert.That(TestSingleton.Instance, Is.EqualTo(singleton1));
    }

    [Test]
    public void Instance_WhenInitialized_PerformsUnderOneMillisecond()
    {
        // Arrange
        var stopwatch = new Stopwatch();
        Object.DestroyImmediate(FindObjectOfType<TestSingleton>()?.gameObject);

        // Act
        stopwatch.Start();
        var instance = TestSingleton.Instance;
        stopwatch.Stop();

        // Assert
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(1), "Singleton initialization took longer than 1ms");
        Assert.That(instance, Is.Not.Null);
    }

    [Test]
    public void Instance_WhenApplicationQuits_ReturnsNull()
    {
        // Arrange
        var gameObject = new GameObject();
        var singleton = gameObject.AddComponent<TestSingleton>();
        
        // Act
        singleton.SendMessage("OnApplicationQuit");
        var instance = TestSingleton.Instance;
        
        // Assert
        Assert.That(instance, Is.Null, "Instance should be null after application quit");
    }

    [Test]
    public void Instance_WhenCreated_SetsDontDestroyOnLoad()
    {
        // Arrange & Act
        var instance = TestSingleton.Instance;
        var sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
        
        // Assert
        // Objects marked with DontDestroyOnLoad are moved to a special scene
        Assert.That(instance.gameObject.scene.buildIndex, Is.EqualTo(-1), 
            "Singleton should be marked with DontDestroyOnLoad");
    }

    [TearDown]
    public void Cleanup()
    {
        var singleton = Object.FindObjectOfType<TestSingleton>();
        if (singleton != null)
        {
            Object.DestroyImmediate(singleton.gameObject);
        }
    }
}
