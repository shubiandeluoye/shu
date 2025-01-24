using UnityEngine;
using GameModule.Core.Systems;
using GameModule.Core.Events;
using GameModule.Unity.Configs;
using Core.EventSystem;
using Fusion;
using GameModule.Unity.Network;

namespace GameModule.Unity.Components
{
    public class GameController : NetworkBehaviour
    {
        [SerializeField] private GameConfigSO configSO;
        
        private GameManager gameManager;
        private UnityEventSystem eventSystem;

        private void Start()
        {
            if (Runner == null) return;
            
            var networkAuthority = new FusionNetworkAuthority(Runner, Object);
            eventSystem = new UnityEventSystem();
            gameManager = new GameManager(eventSystem, networkAuthority);
            gameManager.Initialize(configSO.ToGameConfig());
        }

        private void Update()
        {
            gameManager?.Update(Time.deltaTime);
        }

        private void OnDestroy()
        {
            gameManager?.Dispose();
        }
    }

    // Unity事件系统的实现
    public class UnityEventSystem : IEventSystem
    {
        public void Subscribe<T>(System.Action<T> handler)
        {
            EventManager.Instance.AddListener(handler);
        }

        public void Unsubscribe<T>(System.Action<T> handler)
        {
            EventManager.Instance.RemoveListener(handler);
        }

        public void Publish<T>(T eventData)
        {
            EventManager.Instance.TriggerEvent(eventData);
        }
    }
} 