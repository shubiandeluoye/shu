using UnityEngine;
using Core.Singleton;
using Core.EventSystem;
using System.Collections.Generic;
using Core.Network;
using Core.FSM;
using MapModule.Systems;
using MapModule.Shapes;
using SkillModule;
using MapModule.Utils;
using MapModule.States;
using GameModule;
    
namespace MapModule
{
    /// <summary>
    /// 地图系统总管理器
    /// 职责：
    /// 1. 管理中心区域形状的生成和切换
    /// 2. 处理形状的碰撞检测
    /// 3. 维护形状状态
    /// </summary>
    public class MapSystemManager : Singleton<MapSystemManager>  // 移除网络接口
    {
        #region 内部系统引用
        private StateMachine stateMachine;
        private ShapeFactory shapeFactory;
        private CentralAreaSystem centralAreaSystem;
        private ShapeSystem shapeSystem;
        #endregion

        private Dictionary<int, BaseShape> activeShapes = new Dictionary<int, BaseShape>();
        private Dictionary<ShapeType, List<BaseShape>> shapesByType = new Dictionary<ShapeType, List<BaseShape>>();
        
        [SerializeField] private MapConfig mapConfig;
        [SerializeField] private bool enableDebug = false;

        // 改用普通属性
        private bool isInitialized;
        private int currentShapeType;
        private Vector3 targetPosition;

        // 使用完全限定名称来避免歧义
        private MapModule.Shapes.ShapeState _currentState;

        protected override void OnAwake()
        {
            base.OnAwake();
            MapDebugger.EnableDebug(enableDebug);
            InitializeModule();
        }

        private void InitializeModule()
        {
            shapeFactory = new ShapeFactory(mapConfig);
            centralAreaSystem = new CentralAreaSystem(mapConfig);
            shapeSystem = new ShapeSystem(mapConfig, shapeFactory, centralAreaSystem);
            InitializeStateMachine();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            centralAreaSystem?.Dispose();
            shapeSystem?.Dispose();
        }

        public void UpdateShapes()  // 替换 FixedUpdateNetwork
        {
            foreach (var shape in activeShapes.Values)
            {
                if (shape.IsActive())
                {
                    var state = shape.GetState();
                    HandleShapeState(state);
                }
            }
        }

        private void HandleShapeState(MapModule.Shapes.ShapeState state)
        {
            // 本地事件保持不变
            EventManager.Instance.TriggerEvent(new MapModule.Shapes.ShapeStateChangedEvent 
            { 
                OldState = null,
                NewState = state 
            });

            // 修改网络事件，直接使用状态的属性
            EventManager.Instance.TriggerEvent(new Core.Network.NetworkShapeStateEvent 
            { 
                ShapeId = state.ShapeId,
                Position = state.Position,
                Rotation = Quaternion.Euler(0, 0, state.Rotation),
                Scale = state.Scale
            });
        }

        #region 形状管理
        public void RegisterShape(BaseShape shape)
        {
            if (shape == null) return;
            
            var shapeType = shape.GetShapeType();
            if (!shapesByType.ContainsKey(shapeType))
            {
                shapesByType[shapeType] = new List<BaseShape>();
            }
            shapesByType[shapeType].Add(shape);
        }

        public void UnregisterShape(BaseShape shape)
        {
            if (shape == null) return;
            
            var shapeType = shape.GetShapeType();
            if (shapesByType.ContainsKey(shapeType))
            {
                shapesByType[shapeType].Remove(shape);
                if (shapesByType[shapeType].Count == 0)
                {
                    shapesByType.Remove(shapeType);
                }
            }
        }
        #endregion

        private void OnSystemInitialized()
        {
            isInitialized = true;
            EventManager.Instance.TriggerEvent(new MapSystemInitializedEvent());
        }

        private void InitializeStateMachine()
        {
            stateMachine = new StateMachine();
            
            // 添加地图系统的状态
            stateMachine.AddState<MapInitializingState>(new MapInitializingState());
            stateMachine.AddState<MapPlayingState>(new MapPlayingState());
            stateMachine.AddState<MapPausedState>(new MapPausedState());
            
            // 添加状态转换
            stateMachine.AddTransition<MapInitializingState, MapPlayingState>(
                condition: () => isInitialized,
                delay: 0f,
                onTransition: () => Debug.Log("Map system entering playing state")
            );
            
            stateMachine.AddTransition<MapPlayingState, MapPausedState>(
                condition: () => GameManager.Instance.IsGamePaused,
                delay: 0f
            );
            
            // 设置初始状态
            stateMachine.ChangeState<MapInitializingState>();
            
            // 系统初始化完成
            OnSystemInitialized();
        }
    }
} 