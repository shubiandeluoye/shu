using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;
using System.Diagnostics;
using System.Collections;
using Core.EventSystem;
using Debug = UnityEngine.Debug;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Tests.EditMode.Core
{
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
            Debug.Log("\n=== 开始测试：异常处理器测试 ===");
            
            // 设置预期的日志
            LogAssert.Expect(LogType.Warning, new Regex(".*预期的测试异常.*"));
            
            var eventManager = EventManager.Instance;
            var testData = new TestEventData { Message = "Test" };
            var handlerExecutionOrder = new List<string>();

            Debug.Log("[Test] 添加第一个处理器（将抛出异常）");
            eventManager.AddListener<TestEventData>(data =>
            {
                handlerExecutionOrder.Add("handler1");
                Debug.Log("[Test] 第一个处理器执行，即将抛出异常");
                throw new Exception("预期的测试异常");
            });

            Debug.Log("[Test] 添加第二个处理器");
            eventManager.AddListener<TestEventData>(data =>
            {
                handlerExecutionOrder.Add("handler2");
                Debug.Log("[Test] 第二个处理器执行");
            });

            Debug.Log("[Test] 添加第三个处理器");
            eventManager.AddListener<TestEventData>(data =>
            {
                handlerExecutionOrder.Add("handler3");
                Debug.Log("[Test] 第三个处理器执行");
            });

            Debug.Log("[Test] 触发事件");
            var ex = Assert.Throws<Exception>(() => eventManager.TriggerEvent(testData));
            
            Debug.Log("[Test] 验证结果");
            Assert.That(ex.Message, Is.EqualTo("预期的测试异常"), "应该抛出预期的异常");
            Assert.That(handlerExecutionOrder, Is.EqualTo(new[] { "handler1", "handler2", "handler3" }), 
                "所有处理器应该按顺序执行");
            
            Debug.Log("=== 测试完成 ===\n");
        }

        [Test]
        public void TriggerEvent_Performance_CompletesUnderFiveMilliseconds()
        {
            // Arrange
            var testData = new TestEventData { Message = "Test" };
            Action<TestEventData> handler = (data) => { };  // 移除Sleep
            _eventManager.AddListener<TestEventData>(handler);
            var stopwatch = new Stopwatch();

            // Act
            stopwatch.Start();
            for (int i = 0; i < 1000; i++)  // 增加测试次数来获得更准确的平均值
            {
                _eventManager.TriggerEvent(testData);
            }
            stopwatch.Stop();

            // Assert
            var averageTime = stopwatch.ElapsedMilliseconds / 1000.0;
            Assert.That(averageTime, Is.LessThan(5), $"Event triggering took {averageTime}ms per event");
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
            Debug.Log("\n=== 开始测试：应用退出清理测试 ===");
            
            // 设置预期的日志
            LogAssert.Expect(LogType.Log, "[EventManager] 应用退出，清理所有事件");
            LogAssert.Expect(LogType.Log, "[EventManager] 清理完成");
            
            var eventManager = EventManager.Instance;
            var testData = new TestEventData { Message = "Test" };
            var handlerCalled = false;

            // 添加测试监听器
            eventManager.AddListener<TestEventData>(data => handlerCalled = true);
            Assert.That(eventManager.GetListenerCount<TestEventData>(), Is.EqualTo(1), 
                "应该有一个监听器");

            // 模拟应用退出
            var quitMethod = typeof(EventManager).GetMethod("OnApplicationQuit", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            quitMethod?.Invoke(eventManager, null);

            // 触发事件验证监听器已被清理
            eventManager.TriggerEvent(testData);
            Assert.That(handlerCalled, Is.False, "退出后监听器不应被调用");
            Assert.That(eventManager.GetListenerCount<TestEventData>(), Is.EqualTo(0), 
                "退出后不应有监听器");
            
            Debug.Log("=== 测试完成 ===\n");
        }

        [Test]
        public void EventManager_OnQuit_ShouldCleanupAllEvents()
        {
            // Arrange
            var eventManager = EventManager.Instance;
            eventManager.StartListening("TestEvent", () => { });

            // Act
            eventManager.CleanupOnQuit();

            // Assert
            Assert.That(eventManager.HasListeners("TestEvent"), Is.False);
        }

        [TearDown]
        public void Cleanup()
        {
            if (_eventManagerObject != null)
                UnityEngine.Object.DestroyImmediate(_eventManagerObject);
            // 清理所有监听器
            EventManager.Instance.ClearAllListeners();
            // 重置 LogAssert
            LogAssert.ignoreFailingMessages = false;
        }
    }
}
