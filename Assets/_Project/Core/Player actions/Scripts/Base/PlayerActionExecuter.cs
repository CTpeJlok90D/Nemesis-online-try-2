using System;
using System.Collections.Generic;
using System.Linq;
using Codice.CM.Client.Differences.Graphic;
using Core.ActionsCards;
using Core.Maps;
using Core.Players;
using Core.PlayerTablets;
using Core.Selection.Cards;
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

        [Inject] private RoomsSelection _roomSelection;

        [Inject] private CardsSelection _cardsSelection;

        [Inject] private PlayerTabletList _playerTabletList;

        private NetworkList<NetworkObjectReference> _roomsSelectionNet;

        private NetScriptableObjectList4096<ActionCard> _selectionActionCards;

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
            _roomsSelectionNet = new(writePerm: NetworkVariableWritePermission.Owner);
            _selectionActionCards = new(writePermission: NetworkVariableWritePermission.Owner);
        }

        protected override void OnOwnershipChanged(ulong previous, ulong current)
        {
            base.OnOwnershipChanged(previous, current);
            _executer = _playerTabletList.First(x => x.Player.OwnerClientId == current);
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
                
                gameAction.Inititalize(_executer);

                if (gameAction is INeedMap iNeedMap)
                {
                    iNeedMap.Initialzie(_map);
                }

                if (gameAction is IGameActionWithPayment gameActionWithPayment)
                {
                    int requaredPaymentCount = gameActionWithPayment.RequaredPaymentCount;
                    IReadOnlyCollection<ActionCard> hand = await _executer.ActionCardsDeck.GetHand();

                    ActionCard[] selectedCards = await _cardsSelection.SelectFrom(hand, requaredPaymentCount);
                    _selectionActionCards.SetElements(selectedCards);
                }

                if (gameAction is IGameActionWithRoomsSelection gameActionWithRoomsSelection)
                {
                    RoomCell[] selectedRooms = await _roomSelection.SelectFrom(gameActionWithRoomsSelection.SelectionSource, gameActionWithRoomsSelection.RequredRoomsCount);
                    
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
                
                gameAction.Inititalize(_executer);

                if (gameAction is INeedMap gameActionWithMap)
                {
                    gameActionWithMap.Initialzie(_map);
                }

                if (gameAction is IGameActionWithPayment gameActionWithPayment)
                {
                    while (_selectionActionCards.Count != gameActionWithPayment.RequaredPaymentCount)
                    {
                        await Awaitable.NextFrameAsync();
                    }
                    
                    ActionCard[] cards = await _selectionActionCards.GetElements();
                    _executer.ActionCardsDeck.DiscardCards(cards);
                }

                if (gameAction is IGameActionWithRoomsSelection gameActionWithRoomsSelection)
                {
                    while (_roomsSelectionNet.Count != gameActionWithRoomsSelection.RequredRoomsCount)
                    {
                        await Awaitable.NextFrameAsync();
                    }
                    
                    RoomCell[] selection = _roomsSelectionNet.ToEnumerable().Select(x => 
                    {
                        x.TryGet(out NetworkObject value);
                        return value.GetComponent<RoomCell>();
                    }).ToArray();
                    
                    gameActionWithRoomsSelection.Selection = selection;
                }
                
                gameAction.Execute();
                OnExecuted_RPC();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        [Rpc(SendTo.Owner)]
        private void OnExecuted_RPC()
        {
            _roomsSelectionNet.Clear();
            _selectionActionCards.Clear();
        }
    }
}
