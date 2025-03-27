using System;
using System.Collections.Generic;
using System.Linq;
using Core.ActionsCards;
using Core.Maps;
using Core.PlayerTablets;
using Core.Selection.Cards;
using Core.Selection.Rooms;
using Core.Selection.Tunnels;
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

        [Inject] private NoiseContainerSelection _noiseContainerSelection;

        [Inject] private PlayerTabletList _playerTabletList;

        private NetworkList<NetworkObjectReference> _roomsSelectionNet;
        
        private NetworkList<NetworkObjectReference> _noiseContainerSelectionNet;

        private NetScriptableObjectList4096<ActionCard> _selectionActionCards;

        private PlayerTablet _executer;
        
        private NetVariable<bool> _actionIsExecuting;

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
            _noiseContainerSelectionNet = new(writePerm: NetworkVariableWritePermission.Owner);
            _actionIsExecuting = new(writePerm: NetworkVariableWritePermission.Owner);
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
                if (_actionIsExecuting.Value)
                {
                    throw new InvalidOperationException("Cant execute action: other action is executing");
                }

                _actionIsExecuting.Value = true;
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

                    if (selectedCards.Length != requaredPaymentCount)
                    {
                        _actionIsExecuting.Value = false;
                        return;
                    }
                    
                    _selectionActionCards.SetElements(selectedCards);
                }

                if (gameAction is IGameActionWithRoomsSelection gameActionWithRoomsSelection)
                {
                    RoomCell[] selectedRooms = await _roomSelection.SelectFrom(gameActionWithRoomsSelection.RoomSelectionSource, gameActionWithRoomsSelection.RequredRoomsCount);

                    if (selectedRooms.Length != gameActionWithRoomsSelection.RequredRoomsCount)
                    {
                        _actionIsExecuting.Value = false;
                        return;
                    }
                    
                    _roomsSelectionNet.Clear();
                    foreach (RoomCell roomCell in selectedRooms)
                    {
                        _roomsSelectionNet.Add(roomCell.NetworkObject);
                    }
                    gameActionWithRoomsSelection.RoomSelection = selectedRooms;
                }

                if (gameAction is INeedNoiseContainers needTunnels)
                {
                    INoiseContainer[] selection = await _noiseContainerSelection.SelectFrom(needTunnels.NoiseContainerSelectionSource, needTunnels.RequiredNoiseContainerCount);
                    needTunnels.SelectedNoiseContainers = selection;

                    _noiseContainerSelectionNet.Clear();
                    foreach (INoiseContainer noiseContainer in selection)
                    {
                        _noiseContainerSelectionNet.Add(noiseContainer.NetworkObject);
                    }
                }
                
                Execute_RPC(gameActionContainer);
                _actionIsExecuting.Value = false;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                _actionIsExecuting.Value = false;
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
                        if (_actionIsExecuting.Value == false)
                        {
                            return;
                        }
                        
                        await Awaitable.NextFrameAsync();
                    }
                    
                    ActionCard[] cards = await _selectionActionCards.GetElements();
                    _executer.ActionCardsDeck.DiscardCards(cards);
                }

                if (gameAction is IGameActionWithRoomsSelection gameActionWithRoomsSelection)
                {
                    while (_roomsSelectionNet.Count != gameActionWithRoomsSelection.RequredRoomsCount)
                    {
                        if (_actionIsExecuting.Value == false)
                        {
                            return;
                        }
                        
                        await Awaitable.NextFrameAsync();
                    }
                    
                    RoomCell[] selection = _roomsSelectionNet.ToEnumerable().Select(x => 
                    {
                        x.TryGet(out NetworkObject value);
                        return value.GetComponent<RoomCell>();
                    }).ToArray();
                    
                    gameActionWithRoomsSelection.RoomSelection = selection;
                }

                if (gameAction is INeedNoiseContainers needTunnels)
                {
                    while (_noiseContainerSelectionNet.Count != needTunnels.RequiredNoiseContainerCount)
                    {
                        if (_actionIsExecuting.Value == false)
                        {
                            return;
                        }
                        
                        await Awaitable.NextFrameAsync();
                    }
                    
                    needTunnels.SelectedNoiseContainers = _noiseContainerSelectionNet.ToEnumerable().Select(x => 
                    {
                        x.TryGet(out NetworkObject value);
                        return value.GetComponent<INoiseContainer>();
                    }).ToArray();
                }
                
                gameAction.Execute();
                ClearData_RPC();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        [Rpc(SendTo.Owner)]
        private void ClearData_RPC()
        {
            _roomsSelectionNet.Clear();
            _selectionActionCards.Clear();
            _noiseContainerSelectionNet.Clear();
        }
    }
}
