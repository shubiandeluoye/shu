using UnityEngine;
using Core.FSM;

namespace MapModule.States
{
    public class MapPausedState : IState
    {
        public void Enter()
        {
            Debug.Log("Entering Map Paused State");
            Time.timeScale = 0f;  // 暂停游戏
        }

        public void Exit()
        {
            Debug.Log("Exiting Map Paused State");
            Time.timeScale = 1f;  // 恢复游戏
        }

        public void Update()
        {
            // 暂停状态的更新逻辑
        }

        public bool CanEnter() => true;
        public bool CanExit() => true;
        public void OnSuspend() { }
        public void OnResume() { }
        public string GetStateName() => "MapPaused";
        public StateStatus GetStatus() => StateStatus.Active;
    }
} 