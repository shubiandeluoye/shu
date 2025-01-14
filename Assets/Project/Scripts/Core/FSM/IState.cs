using UnityEngine;

namespace Core.FSM
{
    public interface IState
    {
        void Enter();
        void Update();
        void Exit();
    }
}
