namespace MapModule.Core.States
{
    public class MapInitializingState : MapStateBase
    {
        public MapInitializingState(IMapManager manager) : base(manager) { }

        public override void Enter()
        {
            manager.PublishEvent("StateChanged", "Initializing");
        }

        public override void Exit() { }

        public override void Update(float deltaTime) { }

        public override string GetStateName() => "Initializing";
    }
} 