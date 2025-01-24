using System;

namespace MapModule.Core.States
{
    public abstract class MapStateBase
    {
        protected IMapManager manager;

        public MapStateBase(IMapManager manager)
        {
            this.manager = manager;
        }

        public abstract void Enter();
        public abstract void Exit();
        public abstract void Update(float deltaTime);
        public virtual bool CanEnter() => true;
        public virtual bool CanExit() => true;
        public virtual void OnSuspend() { }
        public virtual void OnResume() { }
        public abstract string GetStateName();
    }
} 