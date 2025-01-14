using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Singleton;
using Core.FSM;
using Core.EventSystem;

namespace Core.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        private StateMachine gameStateMachine;
        public GameState CurrentGameState { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            InitializeStateMachine();
        }

        private void InitializeStateMachine()
        {
            gameStateMachine = new StateMachine();
            gameStateMachine.AddState(new MainMenuState());
            gameStateMachine.AddState(new PlayingState());
            gameStateMachine.AddState(new PausedState());
            gameStateMachine.AddState(new GameOverState());
        }

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

    public class MainMenuState : IState
    {
        public void Enter() { }
        public void Update() { }
        public void Exit() { }
    }

    public class PlayingState : IState
    {
        public void Enter() { }
        public void Update() { }
        public void Exit() { }
    }

    public class PausedState : IState
    {
        public void Enter() { }
        public void Update() { }
        public void Exit() { }
    }

    public class GameOverState : IState
    {
        public void Enter() { }
        public void Update() { }
        public void Exit() { }
    }
}
