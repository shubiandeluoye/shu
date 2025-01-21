using UnityEngine;
using Core.FSM;

namespace MapModule.States
{
    public class MapPlayingState : IState
    {
        public void Enter()
        {
            Debug.Log("Entering Map Playing State");
        }

        public void Exit()
        {
            Debug.Log("Exiting Map Playing State");
        }

        public void Update()
        {
            // 游戏进行状态的更新逻辑
        }

        public bool CanEnter() => true;
        public bool CanExit() => true;
        public void OnSuspend() { }
        public void OnResume() { }
        public string GetStateName() => "MapPlaying";
        public StateStatus GetStatus() => StateStatus.Active;
    }
} 