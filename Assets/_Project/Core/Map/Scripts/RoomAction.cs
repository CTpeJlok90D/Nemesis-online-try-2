using UnityEngine;
using Core.PlayerActions;
using Core.PlayerTablets;

namespace Core.Maps
{
    [Icon("Assets/_Project/Core/Map/Editor/icons8-room-action-96.png")]
    public abstract class RoomAction : ScriptableObject, IGameAction
    {
        public PlayerTablet Executor { get; private set; }
        public RoomCell Room => Executor.CharacterPawn.RoomContent.Owner;

        public void Initialize(PlayerTablet executor)
        {
            Executor = executor;
            OnInitialize();
        }
        
        protected virtual void OnInitialize(){}
        
        public abstract IGameAction.CanExecuteCheckResult CanExecute();
        public abstract void Execute();
        public abstract void ForceExecute();
    }
}
