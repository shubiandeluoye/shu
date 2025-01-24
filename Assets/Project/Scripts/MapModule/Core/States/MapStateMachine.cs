using System.Collections.Generic;

namespace MapModule.Core.States
{
    public class MapStateMachine
    {
        private Dictionary<string, MapStateBase> states;
        private MapStateBase currentState;
        private readonly IMapManager manager;

        public MapStateMachine(IMapManager manager)
        {
            this.manager = manager;
            states = new Dictionary<string, MapStateBase>
            {
                { "Initializing", new MapInitializingState(manager) },
                { "Playing", new MapPlayingState(manager) },
                { "Paused", new MapPausedState(manager) }
            };
        }

        public void ChangeState(string stateName)
        {
            if (!states.ContainsKey(stateName)) return;
            
            var newState = states[stateName];
            if (currentState != null)
            {
                if (!currentState.CanExit()) return;
                currentState.Exit();
            }

            if (!newState.CanEnter()) return;
            
            currentState = newState;
            currentState.Enter();
        }

        public void Update(float deltaTime)
        {
            currentState?.Update(deltaTime);
        }

        public string GetCurrentStateName()
        {
            return currentState?.GetStateName() ?? "None";
        }
    }
} 