using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class FSMTests
{
    private StateMachine _stateMachine;
    private TestState _stateA;
    private TestState _stateB;

    private class TestState : IState
    {
        public bool EnterCalled { get; private set; }
        public bool UpdateCalled { get; private set; }
        public bool ExitCalled { get; private set; }

        public void Enter() => EnterCalled = true;
        public void Update() => UpdateCalled = true;
        public void Exit() => ExitCalled = true;

        public void Reset()
        {
            EnterCalled = false;
            UpdateCalled = false;
            ExitCalled = false;
        }
    }

    [SetUp]
    public void Setup()
    {
        _stateMachine = new StateMachine();
        _stateA = new TestState();
        _stateB = new TestState();
        _stateMachine.AddState(_stateA);
        _stateMachine.AddState(_stateB);
    }

    [Test]
    public void AddState_WhenStateAlreadyExists_LogsWarning()
    {
        // Arrange
        LogAssert.Expect(LogType.Warning, $"[StateMachine] State {typeof(TestState)} already exists.");

        // Act
        _stateMachine.AddState(new TestState());
    }

    [Test]
    public void ChangeState_WhenStateExists_TransitionsCorrectly()
    {
        // Act
        _stateMachine.ChangeState<TestState>();

        // Assert
        Assert.That(_stateA.EnterCalled, Is.True, "Enter should be called on new state");
        Assert.That(_stateA.ExitCalled, Is.False, "Exit should not be called on new state");
    }

    [Test]
    public void ChangeState_WhenChangingStates_CallsExitOnPreviousState()
    {
        // Arrange
        _stateMachine.ChangeState<TestState>();
        _stateA.Reset();

        // Act
        _stateMachine.ChangeState<TestState>();

        // Assert
        Assert.That(_stateA.ExitCalled, Is.True, "Exit should be called on previous state");
        Assert.That(_stateB.EnterCalled, Is.True, "Enter should be called on new state");
    }

    [Test]
    public void Update_WhenStateExists_CallsUpdateOnCurrentState()
    {
        // Arrange
        _stateMachine.ChangeState<TestState>();
        _stateA.Reset();

        // Act
        _stateMachine.Update();

        // Assert
        Assert.That(_stateA.UpdateCalled, Is.True, "Update should be called on current state");
    }

    [Test]
    public void RevertToPreviousState_WhenHistoryExists_RestoresPreviousState()
    {
        // Arrange
        _stateMachine.ChangeState<TestState>();
        var firstState = _stateA;
        _stateMachine.ChangeState<TestState>();
        firstState.Reset();

        // Act
        _stateMachine.RevertToPreviousState();

        // Assert
        Assert.That(firstState.EnterCalled, Is.True, "Enter should be called on reverted state");
        Assert.That(_stateB.ExitCalled, Is.True, "Exit should be called on current state");
    }

    [Test]
    public void GetState_WhenStateExists_ReturnsCorrectState()
    {
        // Act
        var state = _stateMachine.GetState<TestState>();

        // Assert
        Assert.That(state, Is.EqualTo(_stateA));
    }

    [Test]
    public void GetState_WhenStateDoesNotExist_ReturnsNull()
    {
        // Arrange
        public class NonExistentState : IState
        {
            public void Enter() { }
            public void Update() { }
            public void Exit() { }
        }

        // Act
        var state = _stateMachine.GetState<NonExistentState>();

        // Assert
        Assert.That(state, Is.Null);
    }

    [Test]
    public void ChangeState_WhenStateDoesNotExist_LogsError()
    {
        // Arrange
        public class NonExistentState : IState
        {
            public void Enter() { }
            public void Update() { }
            public void Exit() { }
        }

        // Arrange
        LogAssert.Expect(LogType.Error, $"[StateMachine] State {typeof(NonExistentState)} not found.");

        // Act
        _stateMachine.ChangeState<NonExistentState>();
    }
}
