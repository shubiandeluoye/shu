namespace MapModule.Core.States
{
    public class MapPausedState : MapStateBase
    {
        public MapPausedState(IMapManager manager) : base(manager) { }

        public override void Enter()
        {
            manager.PublishEvent("StateChanged", "Paused");
        }

        public override void Exit() { }

        public override void Update(float deltaTime) { }

        public override string GetStateName() => "Paused";
    }
} 