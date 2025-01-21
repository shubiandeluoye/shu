using UnityEngine;
using Core.FSM;

namespace MapModule.States
{
    public class MapInitializingState : IState
    {
        public void Enter()
        {
            Debug.Log("Entering Map Initializing State");
        }

        public void Exit()
        {
            Debug.Log("Exiting Map Initializing State");
        }

        public void Update()
        {
            // 初始化状态的更新逻辑
        }

        public bool CanEnter() => true;
        public bool CanExit() => true;
        public void OnSuspend() { }
        public void OnResume() { }
        public string GetStateName() => "MapInitializing";
        public StateStatus GetStatus() => StateStatus.Active;
    }
} 