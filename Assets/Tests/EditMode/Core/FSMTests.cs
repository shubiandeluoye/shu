using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Core.FSM;

namespace Tests.EditMode.Core
{
    [TestFixture]
    public class FSMTests
    {
        private StateMachine _stateMachine;
        private StateA _stateA;
        private StateB _stateB;

        private class StateA : IState
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

        private class StateB : IState
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

        private class NonExistentState : IState
        {
            public void Enter() { }
            public void Update() { }
            public void Exit() { }
        }

        [SetUp]
        public void Setup()
        {
            _stateMachine = new StateMachine();
            _stateA = new StateA();
            _stateB = new StateB();
            _stateMachine.AddState(_stateA);
            _stateMachine.AddState(_stateB);
        }

        [Test]
        public void AddState_WhenStateAlreadyExists_LogsWarning()
        {
            LogAssert.Expect(LogType.Warning, $"[StateMachine] State {typeof(StateA)} already exists.");
            _stateMachine.AddState(new StateA());
        }

        [Test]
        public void ChangeState_WhenStateExists_TransitionsCorrectly()
        {
            _stateMachine.ChangeState<StateA>();
            Assert.That(_stateA.EnterCalled, Is.True);
            Assert.That(_stateA.ExitCalled, Is.False);
        }

        [Test]
        public void ChangeState_WhenChangingStates_CallsExitOnPreviousState()
        {
            _stateMachine.ChangeState<StateA>();
            _stateA.Reset();
            _stateB.Reset();
            
            _stateMachine.ChangeState<StateB>();
            
            Assert.That(_stateA.ExitCalled, Is.True, "Exit should be called on previous state");
            Assert.That(_stateB.EnterCalled, Is.True, "Enter should be called on new state");
        }

        [Test]
        public void Update_WhenStateExists_CallsUpdateOnCurrentState()
        {
            _stateMachine.ChangeState<StateA>();
            _stateA.Reset();

            _stateMachine.Update();

            Assert.That(_stateA.UpdateCalled, Is.True, "Update should be called on current state");
        }

        [Test]
        public void RevertToPreviousState_WhenHistoryExists_RestoresPreviousState()
        {
            _stateMachine.ChangeState<StateA>();
            _stateMachine.ChangeState<StateB>();
            _stateA.Reset();
            _stateB.Reset();

            _stateMachine.RevertToPreviousState();

            Assert.That(_stateA.EnterCalled, Is.True, "Enter should be called on reverted state");
            Assert.That(_stateB.ExitCalled, Is.True, "Exit should be called on current state");
        }

        [Test]
        public void GetState_WhenStateExists_ReturnsCorrectState()
        {
            var state = _stateMachine.GetState<StateA>();
            Assert.That(state, Is.EqualTo(_stateA));
        }

        [Test]
        public void GetState_WhenStateDoesNotExist_ReturnsNull()
        {
            var state = _stateMachine.GetState<NonExistentState>();
            Assert.That(state, Is.Null);
        }

        [Test]
        public void ChangeState_WhenStateDoesNotExist_LogsError()
        {
            LogAssert.Expect(LogType.Error, $"[StateMachine] State {typeof(NonExistentState)} not found.");
            _stateMachine.ChangeState<NonExistentState>();
        }
    }
}
