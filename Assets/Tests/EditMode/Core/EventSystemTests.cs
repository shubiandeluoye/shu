using NUnit.Framework;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Collections;
using Debug = UnityEngine.Debug;

[TestFixture]
public class EventSystemTests
{
    private EventManager _eventManager;
    private GameObject _eventManagerObject;

    [SetUp]
    public void Setup()
    {
        _eventManagerObject = new GameObject("EventManager");
        _eventManager = _eventManagerObject.AddComponent<EventManager>();
    }

    private class TestEventData
    {
        public string Message { get; set; }
    }

    [Test]
    public void AddListener_WhenRegistered_IncreasesListenerCount()
    {
        // Arrange
        Action<TestEventData> handler = (data) => { };

        // Act
        _eventManager.AddListener<TestEventData>(handler);

        // Assert
        Assert.That(_eventManager.GetListenerCount<TestEventData>(), Is.EqualTo(1));
    }

    [Test]
    public void RemoveListener_WhenExists_DecreasesListenerCount()
    {
        // Arrange
        Action<TestEventData> handler = (data) => { };
        _eventManager.AddListener<TestEventData>(handler);

        // Act
        _eventManager.RemoveListener<TestEventData>(handler);

        // Assert
        Assert.That(_eventManager.GetListenerCount<TestEventData>(), Is.EqualTo(0));
    }

    [Test]
    public void TriggerEvent_WhenHandlerExists_InvokesHandler()
    {
        // Arrange
        bool handlerCalled = false;
        var testData = new TestEventData { Message = "Test" };
        Action<TestEventData> handler = (data) => { handlerCalled = true; };
        _eventManager.AddListener<TestEventData>(handler);

        // Act
        _eventManager.TriggerEvent(testData);

        // Assert
        Assert.That(handlerCalled, Is.True);
    }

    [Test]
    public void TriggerEvent_WithMultipleHandlers_InvokesAllHandlers()
    {
        // Arrange
        int handlerCallCount = 0;
        var testData = new TestEventData { Message = "Test" };
        Action<TestEventData> handler1 = (data) => { handlerCallCount++; };
        Action<TestEventData> handler2 = (data) => { handlerCallCount++; };
        _eventManager.AddListener<TestEventData>(handler1);
        _eventManager.AddListener<TestEventData>(handler2);

        // Act
        _eventManager.TriggerEvent(testData);

        // Assert
        Assert.That(handlerCallCount, Is.EqualTo(2));
    }

    [Test]
    public void TriggerEvent_WhenHandlerThrows_ContinuesExecutingOtherHandlers()
    {
        // Arrange
        bool secondHandlerCalled = false;
        var testData = new TestEventData { Message = "Test" };
        Action<TestEventData> handler1 = (data) => { throw new Exception("Test exception"); };
        Action<TestEventData> handler2 = (data) => { secondHandlerCalled = true; };
        
        _eventManager.AddListener<TestEventData>(handler1);
        _eventManager.AddListener<TestEventData>(handler2);

        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex(@"\[EventManager\] Error triggering event.*"));

        // Act
        _eventManager.TriggerEvent(testData);

        // Assert
        Assert.That(secondHandlerCalled, Is.True);
    }

    [Test]
    public void TriggerEvent_Performance_CompletesUnderFiveMilliseconds()
    {
        // Arrange
        var testData = new TestEventData { Message = "Test" };
        Action<TestEventData> handler = (data) => { /* Simulate work */ System.Threading.Thread.Sleep(1); };
        _eventManager.AddListener<TestEventData>(handler);
        var stopwatch = new Stopwatch();

        // Act
        stopwatch.Start();
        _eventManager.TriggerEvent(testData);
        stopwatch.Stop();

        // Assert
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(5), "Event triggering took longer than 5ms");
    }

    [Test]
    public void ClearAllListeners_WhenCalled_RemovesAllHandlers()
    {
        // Arrange
        Action<TestEventData> handler = (data) => { };
        _eventManager.AddListener<TestEventData>(handler);

        // Act
        _eventManager.ClearAllListeners();

        // Assert
        Assert.That(_eventManager.GetListenerCount<TestEventData>(), Is.EqualTo(0));
    }

    [Test]
    public void GetListenerCount_WhenTypeHasNoListeners_ReturnsZero()
    {
        // Act & Assert
        Assert.That(_eventManager.GetListenerCount<TestEventData>(), Is.EqualTo(0));
    }

    [Test]
    public void OnApplicationQuit_WhenCalled_ClearsAllListeners()
    {
        // Arrange
        Action<TestEventData> handler = (data) => { };
        _eventManager.AddListener<TestEventData>(handler);

        // Act
        _eventManager.SendMessage("OnApplicationQuit");

        // Assert
        Assert.That(_eventManager.GetListenerCount<TestEventData>(), Is.EqualTo(0));
    }

    [TearDown]
    public void Cleanup()
    {
        if (_eventManagerObject != null)
            UnityEngine.Object.DestroyImmediate(_eventManagerObject);
    }
}
