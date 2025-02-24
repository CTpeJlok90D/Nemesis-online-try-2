using System;
using System.Linq;
using Core.Maps;
using Core.PlayerTablets;
using Core.Selection.Rooms;
using Unity.Netcode;
using Unity.Netcode.Custom;
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

        private NetworkList<NetworkObjectReference> _roomsSelectionNet;

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
            _roomsSelectionNet = new();
        }

        public async void Execute(GameActionContainer gameActionContainer)
        {
            try
            {
                if (IsOwner == false)
                {
                    throw new Exception("Only object owner can execute actions");
                }

                IGameAction gameAction = gameActionContainer.GameAction.Value;

                if (gameAction is IGameActionWithRoomsSelection gameActionWithRoomsSelection)
                {
                    RoomCell[] selectedRooms = await _roomSelection.Select(gameActionWithRoomsSelection.RequredRoomsCount);

                    _roomsSelectionNet.Clear();
                    foreach (RoomCell roomCell in selectedRooms)
                    {
                        _roomsSelectionNet.Add(roomCell.NetworkObject);
                    }
                }

                Execute_RPC(gameActionContainer);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        [Rpc(SendTo.Server)]
        private void Execute_RPC(GameActionContainer gameActionContainer)
        {
            ExecuteAsync_Server(gameActionContainer);
        }

        private async void ExecuteAsync_Server(GameActionContainer gameActionContainer)
        {
            try
            {
                await gameActionContainer.Net.AwaitForLoad();

                IGameAction gameAction = gameActionContainer.GameAction.Value;

                if (gameAction is INeedMap gameActionWithMap)
                {
                    gameActionWithMap.Initialzie(_map);
                }

                if (gameAction is IGameActionWithRoomsSelection gameActionWithRoomsSelection)
                {
                    gameActionWithRoomsSelection.Selection = _roomsSelectionNet.ToEnumerable().Select(x => 
                    {
                        x.TryGet(out NetworkObject value);
                        return value.GetComponent<RoomCell>();
                    }).ToArray();
                }

                gameAction.Inititalize(_executer);
                
                gameAction.Execute();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}
