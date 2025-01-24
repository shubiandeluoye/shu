namespace MapModule.Core.States
{
    public class MapPlayingState : MapStateBase
    {
        public MapPlayingState(IMapManager manager) : base(manager) { }

        public override void Enter()
        {
            manager.PublishEvent("StateChanged", "Playing");
        }

        public override void Exit() { }

        public override void Update(float deltaTime)
        {
            // 更新地图系统
            manager.Update(deltaTime);
        }

        public override string GetStateName() => "Playing";
    }
} 