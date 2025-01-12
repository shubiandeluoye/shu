using UnityEngine;
using UnityEngine.SceneManagement;
using Core;
using Core.FSM;
using Core.EventSystem;

namespace Core.Managers
{
    /// <summary>
    /// 中央游戏管理器
    /// 负责处理游戏状态和流程控制
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        private StateMachine gameStateMachine;
        public GameState CurrentGameState { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            InitializeStateMachine();
        }

        /// <summary>
        /// 初始化状态机并添加基本游戏状态
        /// </summary>
        private void InitializeStateMachine()
        {
            gameStateMachine = new StateMachine();
            
            // 添加游戏状态
            gameStateMachine.AddState(new MainMenuState());
            gameStateMachine.AddState(new PlayingState());
            gameStateMachine.AddState(new PausedState());
            gameStateMachine.AddState(new GameOverState());
        }

        /// <summary>
        /// 改变游戏状态
        /// </summary>
        /// <param name="newState">新的游戏状态</param>
        public void ChangeGameState(GameState newState)
        {
            CurrentGameState = newState;
            switch (newState)
            {
                case GameState.MainMenu:
                    gameStateMachine.ChangeState<MainMenuState>();
                    break;
                case GameState.Playing:
                    gameStateMachine.ChangeState<PlayingState>();
                    break;
                case GameState.Paused:
                    gameStateMachine.ChangeState<PausedState>();
                    break;
                case GameState.GameOver:
                    gameStateMachine.ChangeState<GameOverState>();
                    break;
            }
            
            EventManager.Instance.TriggerEvent(new GameStateChangedEvent(newState));
        }

        /// <summary>
        /// 加载指定场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        public void LoadScene(string sceneName)
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
                    sceneName,
                    new LoadSceneParameters(LoadSceneMode.Single)
                );
                return;
            }
            #endif
            SceneManager.LoadSceneAsync(sceneName);
        }

        private void Update()
        {
            gameStateMachine?.Update();
        }
    }

    /// <summary>
    /// 主菜单状态
    /// </summary>
    public class MainMenuState : IState
    {
        public void Enter() => Debug.Log("进入主菜单状态");
        public void Update() { }
        public void Exit() => Debug.Log("退出主菜单状态");
    }

    /// <summary>
    /// 游戏进行状态
    /// </summary>
    public class PlayingState : IState
    {
        public void Enter() => Debug.Log("进入游戏状态");
        public void Update() { }
        public void Exit() => Debug.Log("退出游戏状态");
    }

    /// <summary>
    /// 游戏暂停状态
    /// </summary>
    public class PausedState : IState
    {
        public void Enter() => Debug.Log("进入暂停状态");
        public void Update() { }
        public void Exit() => Debug.Log("退出暂停状态");
    }

    /// <summary>
    /// 游戏结束状态
    /// </summary>
    public class GameOverState : IState
    {
        public void Enter() => Debug.Log("进入游戏结束状态");
        public void Update() { }
        public void Exit() => Debug.Log("退出游戏结束状态");
    }
}
