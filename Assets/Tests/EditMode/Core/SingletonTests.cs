using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using Core;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using UnityEngine.Profiling;

namespace Tests.EditMode.Core
{
    public class SingletonTests
    {
        private class TestSingleton : Singleton<TestSingleton> { }

        [SetUp]
        public void Setup()
        {
            Debug.Log($"\n=== 测试开始 [{DateTime.Now:HH:mm:ss.fff}] ===");
            TestSingleton.ResetInstance();
        }

        [Test]
        public void Instance_WhenAccessedFirst_CreatesNewInstance()
        {
            Debug.Log($"[Test] {Time.time} 通过Instance属性获取实例");
            var instance = TestSingleton.Instance;
            
            Assert.That(instance, Is.Not.Null, "应该创建新实例");
            Assert.That(instance.name, Is.EqualTo("[TestSingleton]"), "实例应该有正确的名称");
            Assert.That(TestSingleton.CurrentInstance, Is.SameAs(instance), "CurrentInstance应该返回相同的实例");
        }

        [Test]
        public void Instance_WhenPlacedInScene_UsesExistingInstance()
        {
            Debug.Log($"[Test] {Time.time} 在场景中创建实例");
            var go = new GameObject("SceneTestSingleton");
            var sceneInstance = go.AddComponent<TestSingleton>();
            
            // 手动调用Awake以模拟Unity的行为
            var awakeMethod = typeof(TestSingleton).GetMethod("Awake", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            Debug.Log($"[Test] {Time.time} 调用Awake方法");
            awakeMethod?.Invoke(sceneInstance, null);

            // 通过属性获取实例
            Debug.Log($"[Test] {Time.time} 获取Instance属性");
            var instance = TestSingleton.Instance;
            
            Assert.That(instance, Is.SameAs(sceneInstance), "应该使用场景中的实例");
            Assert.That(TestSingleton.CurrentInstance, Is.SameAs(sceneInstance), "CurrentInstance应该返回场景实例");
        }

        [Test]
        public void Instance_WhenMultipleInScene_PreventsDuplicates()
        {
            Debug.Log($"[Test] {Time.time} 测试多个实例的情况");
            
            // 创建第一个实例
            Debug.Log($"[Test] {Time.time} 创建第一个实例");
            var firstGo = new GameObject("FirstSingleton");
            var firstInstance = firstGo.AddComponent<TestSingleton>();
            
            var awakeMethod = typeof(TestSingleton).GetMethod("Awake", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            Debug.Log($"[Test] {Time.time} 调用第一个实例的Awake");
            awakeMethod?.Invoke(firstInstance, null);

            // 确保第一个实例被正确设置
            Assert.That(TestSingleton.CurrentInstance, Is.SameAs(firstInstance), "第一个实例应该成为当前实例");

            // 创建第二个实例
            Debug.Log($"[Test] {Time.time} 创建第二个实例");
            var secondGo = new GameObject("SecondSingleton");
            var secondInstance = secondGo.AddComponent<TestSingleton>();
            
            Debug.Log($"[Test] {Time.time} 调用第二个实例的Awake");
            awakeMethod?.Invoke(secondInstance, null);

            // 验证结果
            var allInstances = GameObject.FindObjectsOfType<TestSingleton>();
            Debug.Log($"[Test] {Time.time} 找到的实例数量: {allInstances.Length}");
            
            Assert.That(allInstances.Length, Is.EqualTo(1), "应该只有一个实例存在");
            Assert.That(TestSingleton.CurrentInstance, Is.SameAs(firstInstance), "第一个实例应该保持为当前实例");
            Assert.That(secondGo == null || !secondGo.activeInHierarchy, "第二个实例应该被销毁");
        }

        [Test]
        public void Instance_WhenAccessedFromMultipleThreads_RemainsSingleton()
        {
            Debug.Log($"[Test] {Time.time} 测试多线程访问");
            
            // 在主线程创建实例
            var mainThreadInstance = TestSingleton.Instance;
            var instances = new List<TestSingleton>();
            var tasks = new List<Task>();

            // 从多个线程访问实例
            for (int i = 0; i < 10; i++)
            {
                var task = Task.Run(() =>
                {
                    var threadInstance = TestSingleton.Instance;
                    lock (instances)
                    {
                        instances.Add(threadInstance);
                    }
                });
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
            
            Debug.Log($"[Test] {Time.time} 验证所有线程获取到的实例");
            Assert.That(instances, Has.All.SameAs(mainThreadInstance), "所有线程应该获取相同的实例");
        }

        [UnityTest]
        public IEnumerator Instance_WhenSceneReloads_MaintainsInstance()
        {
            Debug.Log($"[Test] {Time.time} 测试场景重载");
            
            // 获取初始实例
            var originalInstance = TestSingleton.Instance;
            var originalId = originalInstance.GetInstanceID();

            yield return null;

            // 模拟场景重载
            Debug.Log($"[Test] {Time.time} 创建新场景实例");
            var newGo = new GameObject("NewSceneSingleton");
            var newInstance = newGo.AddComponent<TestSingleton>();
            
            var awakeMethod = typeof(TestSingleton).GetMethod("Awake", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            Debug.Log($"[Test] {Time.time} 调用新实例的Awake");
            awakeMethod?.Invoke(newInstance, null);

            Assert.That(TestSingleton.Instance.GetInstanceID(), Is.EqualTo(originalId), 
                "实例应该在场景重载后保持不变");
        }

        [TearDown]
        public void Cleanup()
        {
            Debug.Log($"[Test] {Time.time} 清理测试环境");
            var instances = GameObject.FindObjectsOfType<TestSingleton>();
            foreach (var instance in instances)
            {
                GameObject.DestroyImmediate(instance.gameObject);
            }
            TestSingleton.ResetInstance();
            Debug.Log($"=== 测试结束 [{DateTime.Now:HH:mm:ss.fff}] ===\n");
        }
    }
}
