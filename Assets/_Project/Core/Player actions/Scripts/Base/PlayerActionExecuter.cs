using System;
using System.Collections.Generic;
using System.Linq;
using Core.ActionsCards;
using Core.CharacterInventories;
using Core.Maps;
using Core.PlayerActions.Base;
using Core.PlayerTablets;
using Core.Selection.Cards;
using Core.Selection.InventoryItems;
using Core.Selection.RoomContentSelections;
using Core.Selection.Rooms;
using Core.Selection.Tunnels;
using Cysharp.Threading.Tasks;
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
        
        [Inject] private RoomContentSelection _roomContentSelection;
        
        [Inject] private InventoryItemsSelection _inventoryItemsSelection;

        private NetworkList<NetworkObjectReference> _roomsSelectionNet;
        
        private NetworkList<NetworkObjectReference> _noiseContainerSelectionNet;
        
        private NetworkList<NetworkObjectReference> _roomContentSelectionNet;

        private NetworkList<NetworkObjectReference> _inventoryItemsSelectionNet;

        private NetScriptableObjectList4096<ActionCard> _selectionActionCards;

        private PlayerTablet _executor;
        
        private NetVariable<bool> _actionIsExecuting;

        public PlayerTablet Executor
        {
            get
            {
                return _executor;
            }
            set
            {
                _executor = value;
                
                ulong playerID = _executor.Player.OwnerClientId;
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
            _roomContentSelectionNet = new(writePerm: NetworkVariableWritePermission.Owner);
            _inventoryItemsSelectionNet = new (writePerm: NetworkVariableWritePermission.Owner);
        }

        protected override void OnOwnershipChanged(ulong previous, ulong current)
        {
            base.OnOwnershipChanged(previous, current);
            _executor = _playerTabletList.First(x => x.Player.OwnerClientId == current);
        }

        public async void Execute(GameActionContainer gameActionContainer)
        {
            try
            {
                if (_actionIsExecuting.Value)
                {
                    throw new InvalidOperationException("Cant execute action: other action is executing");
                }

                if (IsOwner == false)
                {
                    throw new Exception("Only object owner can execute actions");
                }
                
                _actionIsExecuting.Value = true;

                IGameAction gameAction = gameActionContainer.GameAction.Value;
                
                gameAction.Inititalize(_executor);

                if (gameAction is INeedMap iNeedMap)
                {
                    iNeedMap.Initialzie(_map);
                }

                if (gameAction is IGameActionWithPayment gameActionWithPayment)
                {
                    ActionCard[] selection = await gameActionWithPayment.GetSelectionLocal(_executor, _cardsSelection);
                    
                    if (selection.Length != gameActionWithPayment.RequaredPaymentCount)
                    {
                        _actionIsExecuting.Value = false;
                        return;
                    }
                    
                    _selectionActionCards.SetElements(selection);
                }
                
                if (gameAction is IRequireInventoryItems gameActionWithInventoryItem)
                {
                    _inventoryItemsSelectionNet.Clear();
                    InventoryItem[] selection = await gameActionWithInventoryItem.GetSelectionLocal(_inventoryItemsSelection);

                    if (selection.Length != gameActionWithInventoryItem.RequiredItemsAmount)
                    {
                        _actionIsExecuting.Value = false;
                        return;
                    }
                    
                    foreach (InventoryItem instance in selection)
                    {
                        _inventoryItemsSelectionNet.Add(instance.NetworkObject);
                    }
                }

                if (gameAction is IGameActionWithRoomsSelection gameActionWithRoomsSelection)
                {
                    _roomsSelectionNet.Clear();
                    RoomCell[] selectedRooms = await _roomSelection.SelectFrom(gameActionWithRoomsSelection.RoomSelectionSource, gameActionWithRoomsSelection.RequredRoomsCount);

                    if (selectedRooms.Length != gameActionWithRoomsSelection.RequredRoomsCount)
                    {
                        _actionIsExecuting.Value = false;
                        return;
                    }
                    
                    foreach (RoomCell roomCell in selectedRooms)
                    {
                        _roomsSelectionNet.Add(roomCell.NetworkObject);
                    }
                    gameActionWithRoomsSelection.RoomSelection = selectedRooms;
                }

                if (gameAction is INeedNoiseContainers needTunnels)
                {
                    _noiseContainerSelectionNet.Clear();
                    INoiseContainer[] selection = await _noiseContainerSelection.SelectFrom(needTunnels.NoiseContainerSelectionSource, needTunnels.RequiredNoiseContainerCount);

                    if (selection.Length != needTunnels.RequiredNoiseContainerCount)
                    {
                        _actionIsExecuting.Value = false;
                        return;
                    }
                    
                    needTunnels.SelectedNoiseContainers = selection;

                    foreach (INoiseContainer noiseContainer in selection)
                    {
                        _noiseContainerSelectionNet.Add(noiseContainer.NetworkObject);
                    }
                }

                if (gameAction is IGameActionWithRoomContentSelection gameActionWithRoomContentSelection)
                {
                    _roomContentSelectionNet.Clear();
                    RoomContent[] selection = await _roomContentSelection.SelectFrom(gameActionWithRoomContentSelection.RoomContentSelectionSource, gameActionWithRoomContentSelection.RequiredRoomContentCount);
                    
                    if (selection.Length != gameActionWithRoomContentSelection.RequiredRoomContentCount)
                    {
                        _actionIsExecuting.Value = false;
                        return;
                    }
                    
                    gameActionWithRoomContentSelection.RoomContentSelection = selection;
                    
                    foreach (RoomContent roomContent in selection)
                    {
                        _roomContentSelectionNet.Add(roomContent.NetworkObject);
                    }
                }
                
                _actionIsExecuting.Value = false;
                Execute_RPC(gameActionContainer);
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
            _ = ExecuteAsync_Server(gameActionContainer);
        }

        private async UniTask ExecuteAsync_Server(GameActionContainer gameActionContainer)
        {
            await gameActionContainer.Net.AwaitForLoad();
                
            IGameAction gameAction = gameActionContainer.GameAction.Value;
            
            gameAction.Inititalize(_executor);

            if (gameAction is INeedMap gameActionWithMap)
            {
                gameActionWithMap.Initialzie(_map);
            }
            
            if (gameAction is IRequireInventoryItems gameActionWithInventoryItem)
            {
                while (_inventoryItemsSelectionNet.Count != gameActionWithInventoryItem.RequiredItemsAmount)
                {
                    if (_actionIsExecuting.Value == false)
                    {
                        return;
                    }
                    
                    await Awaitable.NextFrameAsync();
                }
                
                gameActionWithInventoryItem.InventoryItemsSelection = _inventoryItemsSelectionNet.ToEnumerable<InventoryItem>().ToArray();
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

            if (gameAction is IGameActionWithRoomContentSelection gameActionWithRoomContentSelection)
            {
                while (gameActionWithRoomContentSelection.RequiredRoomContentCount != _roomContentSelectionNet.Count)
                {
                    if (_actionIsExecuting.Value == false)
                    {
                        return;
                    }
                    
                    await Awaitable.NextFrameAsync();
                }
                
                gameActionWithRoomContentSelection.RoomContentSelection = _roomContentSelectionNet.ToEnumerable().Select(x =>
                {
                    x.TryGet(out NetworkObject value);
                    return value.GetComponent<RoomContent>();
                }).ToArray();
            }

            gameAction.Execute();
            
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
                _executor.ActionCardsDeck.DiscardCards(cards);
            }
            
            ClearData_RPC();
        }

        [Rpc(SendTo.Owner)]
        private void ClearData_RPC()
        {
            _roomsSelectionNet.Clear();
            _selectionActionCards.Clear();
            _noiseContainerSelectionNet.Clear();
            _roomContentSelectionNet.Clear();
            _actionIsExecuting.Value = false;
        }
    }
}
