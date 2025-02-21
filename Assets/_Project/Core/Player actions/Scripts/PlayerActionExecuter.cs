using System;
using Core.Maps;
using Core.PlayerTablets;
using Core.Selection.Rooms;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Core.PlayerActions
{
    public class PlayerActionExecutor : NetworkBehaviour
    {
        public delegate void RoomsSelectionChangedHandler(PlayerActionExecutor sender);

        public static PlayerActionExecutor Instance { get; private set; }

        [SerializeField] private Map _map;

        [Inject] private RoomSelection _roomSelection;

        private PlayerTablet _executer;

        public PlayerTablet Executer
        {
            get
            {
                return _executer;
            }
            set
            {
                _executer = value;
                
                ulong playerID = _executer.Player.OwnerClientId;
                NetworkObject.ChangeOwnership(playerID);
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                enabled = false;
                throw new Exception($"{nameof(PlayerActionExecutor)} is already instantiated!");
            }
            Instance = this;
        }

        public void Execute(GameActionContainer gameActionContainer)
        {
            if (IsOwner == false)
            {
                throw new Exception("Only object owner can execute actions");
            }

            IGameAction gameAction = gameActionContainer.GameAction.Value;
            
            int selectedRoomCount = _roomSelection.Count;

            if (gameAction is IGameActionWithRoomsSelection gameActionWithRoomsSelection 
                && gameActionWithRoomsSelection.RequredRoomsCount != selectedRoomCount)
            {
                throw new Exception("Invalid selected rooms count");
            }

            Execute_RPC(gameActionContainer);
        }

        [Rpc(SendTo.Server)]
        private void Execute_RPC(GameActionContainer gameActionContainer)
        {
            IGameAction gameAction = gameActionContainer.GameAction.Value;

            gameAction.Inititalize(_executer);

            if (gameAction is INeedMap gameActionWithMap)
            {
                gameActionWithMap.Initialzie(_map);
            }

            if (gameAction is IGameActionWithRoomsSelection gameActionWithRoomsSelection)
            {
                gameActionWithRoomsSelection.Initialize(_roomSelection);
            }

            gameAction.Execute();
        }
    }
}
