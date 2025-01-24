using UnityEngine;
using MapModule.Core.Data;
using MapModule.Core.Systems;
using MapModule.Core.Utils;

namespace MapModule.Unity.Components
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private ShapeController shapeControllerPrefab;
        [SerializeField] private Transform shapeContainer;
        
        private MapManager mapManager;
        private ShapeController currentShapeController;

        private void Start()
        {
            InitializeMapSystem();
            SubscribeToEvents();
        }

        private void InitializeMapSystem()
        {
            var config = LoadMapConfig();
            mapManager = MapManager.Instance;
            mapManager.Initialize(config);
        }

        private void SubscribeToEvents()
        {
            MapEventSystem.Instance.Subscribe(MapEvents.ShapeChanged, OnShapeChanged);
            MapEventSystem.Instance.Subscribe(MapEvents.ShapeHit, OnShapeHit);
            MapEventSystem.Instance.Subscribe(MapEvents.ShapeDestroyed, OnShapeDestroyed);
        }

        private void Update()
        {
            TimeUtils.Update(Time.time, Time.deltaTime);
            mapManager?.Update(Time.deltaTime);
            UpdateCurrentShape();
        }

        private void UpdateCurrentShape()
        {
            if (currentShapeController != null)
            {
                var state = mapManager.GetCurrentShapeState();
                if (state != null)
                {
                    currentShapeController.UpdateState(state);
                }
            }
        }

        private void OnShapeChanged(object data)
        {
            var evt = (ShapeChangedEvent)data;
            
            // 清理旧的形状
            if (currentShapeController != null)
            {
                Destroy(currentShapeController.gameObject);
            }

            // 创建新的形状
            currentShapeController = Instantiate(shapeControllerPrefab, shapeContainer);
            currentShapeController.Initialize(evt.Type);
        }

        private void OnShapeHit(object data)
        {
            var evt = (ShapeHitEvent)data;
            currentShapeController?.OnHit(evt.HitPoint);
        }

        private void OnShapeDestroyed(object data)
        {
            var evt = (ShapeDestroyedEvent)data;
            currentShapeController?.OnDestroy();
        }

        private MapConfig LoadMapConfig()
        {
            // TODO: 从ScriptableObject加载配置
            return new MapConfig();
        }

        private void OnDestroy()
        {
            mapManager?.Dispose();
        }
    }
} 