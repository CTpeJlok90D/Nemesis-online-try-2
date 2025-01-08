using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ActionsCards;
using Core.Maps;
using Core.PlayerTablets;
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

        [Inject] private PlayerTabletList _playerTabletList;

        private NetScriptableObjectList4096<ActionCard> _paymentNet;

        private NetworkList<NetworkObjectReference> _roomSelection;

        private PlayerTablet _executer;

        public async Task<IReadOnlyCollection<ActionCard>> GetPayment() => await _paymentNet.GetElements();

        public int PaymentCount => _paymentNet.Count;

        public event RoomsSelectionChangedHandler RoomSelectionChanged;

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

        public event NetScriptableObjectList4096<ActionCard>.ListChangedListener PaymentChanged
        {
            add => _paymentNet.ListChanged += value;
            remove => _paymentNet.ListChanged -= value;
        }

        public RoomCell[] RoomSelection
        {
            get
            {
                List<RoomCell> roomCells = new();
                foreach (NetworkObjectReference networkObjectReference in _roomSelection)
                {
                    if (networkObjectReference.TryGet(out NetworkObject netObj) && netObj.TryGetComponent(out RoomCell roomCell))
                    {
                        roomCells.Add(roomCell);
                    }
                }

                return roomCells.ToArray();
            }
        }

        public void AddPaymentToSelection(ActionCard objectToAdd)
        {
            if (CanAddPaymentToSelection(objectToAdd))
            {
                _paymentNet.Add(objectToAdd);
            }
        }

        public bool RemovePaymentFromSelection(ActionCard objectToAdd)
        {
            return _paymentNet.Remove(objectToAdd);
        }

        public bool CanAddPaymentToSelection(ActionCard actionCard)
        {
            return _paymentNet.Contains(actionCard) == false;
        }

        public bool CanAddRoomToSelection(RoomCell roomCell)
        {
            return RoomSelection.Contains(roomCell) == false;
        }

        public void AddRoomToSelection(RoomCell roomCell)
        {
            if (CanAddRoomToSelection(roomCell) == false)
            {
                throw new Exception("Selection already contains this room cell");
            }

            _roomSelection.Add(roomCell.NetworkObject);
        }

        public bool RemoveRoomFromSelection(RoomCell roomCell)
        {
            return _roomSelection.Remove(roomCell.NetworkObject);
        }

        private void Awake()
        {
            if (Instance != null)
            {
                enabled = false;
                throw new Exception($"{nameof(PlayerActionExecutor)} is already instantiated!");
            }
            Instance = this;

            _paymentNet = new(NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
            _roomSelection = new(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        }

        private void OnEnable()
        {
            _roomSelection.OnListChanged += OnListChange;
        }

        private void OnDisable()
        {
            _roomSelection.OnListChanged -= OnListChange;
        }

        private void OnListChange(NetworkListEvent<NetworkObjectReference> changeEvent)
        {
            RoomSelectionChanged?.Invoke(this);
        }

        public void Execute(GameActionContainer gameActionContainer)
        {
            if (IsOwner == false)
            {
                throw new Exception("Only object owner can execute actions");
            }

            Execute_RPC(gameActionContainer);
        }

        [Rpc(SendTo.Server)]
        private void Execute_RPC(GameActionContainer gameActionContainer)
        {
            IGameAction gameAction = gameActionContainer.GameAction.Value;

            gameAction.Init(_executer);

            if (gameAction is INeedMap gameActionWithMap)
            {
                gameActionWithMap.Init(_map);
            }

            gameAction.Execute();
        }
    }
}
