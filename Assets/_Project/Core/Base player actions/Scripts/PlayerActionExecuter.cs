using System;
using System.Collections.Generic;
using System.Linq;
using Core.ActionsCards;
using Core.Maps;
using Unity.Netcode;
using Unity.Netcode.Custom;

namespace Core.BasePlayerActions
{
    public class PlayerActionExecutor : NetworkBehaviour
    {
        public delegate void RoomsSelectionChangedHandler(PlayerActionExecutor sender);

        public static PlayerActionExecutor Instance { get; private set; }

        internal NetScriptableObjectList4096<ActionCard> PaymentNet { get; private set; }

        private NetworkList<NetworkObjectReference> _roomSelection;

        public IReadOnlyCollection<ActionCard> Payment => PaymentNet.ToArray();

        public event RoomsSelectionChangedHandler RoomSelectionChanged;

        public event NetScriptableObjectList4096<ActionCard>.ListChangedListener PaymentChanged
        {
            add => PaymentNet.ListChanged += value;
            remove => PaymentNet.ListChanged -= value;
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

        internal bool CanAddPaymentToSelection(ActionCard actionCard)
        {
            return PaymentNet.Contains(actionCard) == false;
        }

        internal bool CanAddRoomToSelection(RoomCell roomCell)
        {
            return RoomSelection.Contains(roomCell) == false;
        }

        internal void AddRoomToSelection(RoomCell roomCell)
        {
            if (CanAddRoomToSelection(roomCell) == false)
            {
                throw new Exception("Selection already contains this room cell");
            }

            _roomSelection.Add(roomCell.NetworkObject);
        }

        internal bool RemoveRoomFromSelection(RoomCell roomCell)
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

            PaymentNet = new(NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
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
    }
}
