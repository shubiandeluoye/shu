using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central game manager handling game state and flow control
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

    private void InitializeStateMachine()
    {
        gameStateMachine = new StateMachine();
        
        // Add game states
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
        SceneManager.LoadSceneAsync(sceneName);
    }

    private void Update()
    {
        gameStateMachine?.Update();
    }
}

// Game State implementations
public class MainMenuState : IState
{
    public void Enter() => Debug.Log("Entering Main Menu State");
    public void Update() { }
    public void Exit() => Debug.Log("Exiting Main Menu State");
}

public class PlayingState : IState
{
    public void Enter() => Debug.Log("Entering Playing State");
    public void Update() { }
    public void Exit() => Debug.Log("Exiting Playing State");
}

public class PausedState : IState
{
    public void Enter() => Debug.Log("Entering Paused State");
    public void Update() { }
    public void Exit() => Debug.Log("Exiting Paused State");
}

public class GameOverState : IState
{
    public void Enter() => Debug.Log("Entering Game Over State");
    public void Update() { }
    public void Exit() => Debug.Log("Exiting Game Over State");
}
