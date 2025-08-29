using System;
using System.Collections.Generic;
using System.Linq;
using Core.ActionsCards;
using Core.CharacterInventories;
using Core.DestinationCoordinats;
using Core.LootDecks;
using Core.Maps;
using Core.PlayerActions.Base;
using Core.PlayerTablets;
using Core.Selection.Cards;
using Core.Selection.InventoryItems;
using Core.Selection.LootDeckSelections;
using Core.Selection.RoomContentSelections;
using Core.Selection.Rooms;
using Core.Selection.Tunnels;
using Cysharp.Threading.Tasks;
using UI.Selection;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Core.PlayerActions
{
    public class PlayerActionExecutor : NetworkBehaviour
    {
        public static PlayerActionExecutor Singleton { get; private set; }

        [FormerlySerializedAs("_map")] 
        [SerializeField] private Ship _ship;

        [Inject] private RoomsSelection _roomSelection;
        [Inject] private CardsSelection _cardsSelection;
        [Inject] private NoiseContainerSelection _noiseContainerSelection;
        [Inject] private RoomContentSelection _roomContentSelection;
        [Inject] private InventoryItemsSelection _inventoryItemsSelection;
        [Inject] private LootDeckSelection _lootDeckSelection;
        [Inject] private CoordinatesSelection _coordinatesSelection;

        private NetworkList<NetworkObjectReference> _roomsSelectionNet;
        private NetworkList<NetworkObjectReference> _noiseContainerSelectionNet;
        private NetworkList<NetworkObjectReference> _roomContentSelectionNet;
        private NetworkList<NetworkObjectReference> _inventoryItemsSelectionNet;
        private NetworkList<int> _selectedLootTypesNet;
        private NetScriptableObjectList4096<ActionCard> _selectionActionCardsNet;
        private NetScriptableObjectList4096<Coordinate> _selectionCoordinatesNet;
        private PlayerTablet _executor;
        private NetVariable<bool> _actionIsExecuting;
        
        public IReadOnlyReactiveField<bool> ActionIsExecuting => _actionIsExecuting;

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
            if (Singleton != null)
            {
                enabled = false;
                throw new Exception($"{nameof(PlayerActionExecutor)} is already instantiated!");
            }
            Singleton = this;
            _roomsSelectionNet = new(writePerm: NetworkVariableWritePermission.Owner);
            _selectionActionCardsNet = new(writePerm: NetworkVariableWritePermission.Owner);
            _noiseContainerSelectionNet = new(writePerm: NetworkVariableWritePermission.Owner);
            _actionIsExecuting = new(writePerm: NetworkVariableWritePermission.Owner);
            _roomContentSelectionNet = new(writePerm: NetworkVariableWritePermission.Owner);
            _inventoryItemsSelectionNet = new (writePerm: NetworkVariableWritePermission.Owner);
            _selectedLootTypesNet = new(writePerm: NetworkVariableWritePermission.Owner);
            _selectionCoordinatesNet = new(writePerm: NetworkVariableWritePermission.Owner);
        }

        protected override void OnOwnershipChanged(ulong previous, ulong current)
        {
            base.OnOwnershipChanged(previous, current);
            _executor = PlayerTablet.Instances.First(x => x.Player.OwnerClientId == current);
        }
        
        public async UniTask Execute(GameActionContainer gameActionContainer)
        {
            if (_actionIsExecuting.Value)
            {
                throw new InvalidOperationException("Cant execute action: other action is executing");
            }

            if (IsOwner == false)
            {
                throw new InvalidOperationException("Only object owner can execute actions");
            }
            
            _actionIsExecuting.Value = true;

            IGameAction gameAction = gameActionContainer.GameAction.Value;
            
            gameAction.Initialize(_executor);

            if (gameAction is INeedMap iNeedMap)
            {
                iNeedMap.Initialzie(_ship);
            }
            
            _roomSelection.CanCancel = gameAction.CanCancel;
            _cardsSelection.CanCancel = gameAction.CanCancel;
            _noiseContainerSelection.CanCancel = gameAction.CanCancel;
            _inventoryItemsSelection.CanCancel = gameAction.CanCancel;
            _roomContentSelection.CanCancel = gameAction.CanCancel;
            _lootDeckSelection.CanCancel = gameAction.CanCancel;
            _selectedLootTypesNet.Clear();
            _inventoryItemsSelectionNet.Clear();
            _roomsSelectionNet.Clear();
            _noiseContainerSelectionNet.Clear();
            _roomContentSelectionNet.Clear();
            
            Execute_RPC(gameActionContainer);

            if (gameAction is INeedPayment gameActionWithPayment)
            {
                ActionCard[] selection = await gameActionWithPayment.GetSelectionLocal(_executor, _cardsSelection);
                
                if (selection.Length != gameActionWithPayment.RequaredPaymentCount)
                {
                    _actionIsExecuting.Value = false;
                    return;
                }
                
                _selectionActionCardsNet.SetElements(selection);
            }

            if (gameAction is INeedLootDeck needLootDeck)
            {
                LootDeck.Type[] selectedTypes = await needLootDeck.GetSelectionLocal(_lootDeckSelection);

                if (selectedTypes.Length != needLootDeck.RequiredLootDecksAmount)
                {
                    _actionIsExecuting.Value = false;
                    return;
                }

                foreach (LootDeck.Type selectedType in selectedTypes)
                {
                    _selectedLootTypesNet.Add((int)selectedType);
                }
            }

            if (gameAction is INeedInventoryItems gameActionWithInventoryItem)
            {
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

            if (gameAction is INeedRooms gameActionWithRoomsSelection)
            {
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

            if (gameAction is INeedRoomContents gameActionWithRoomContentSelection)
            {
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

            if (gameAction is INeedCoordinates needCoordinates)
            {
                _selectionCoordinatesNet.Clear();
                Coordinate[] coordinates = await needCoordinates.GetSelectionLocal(_coordinatesSelection);

                needCoordinates.CoordinatesSelection = coordinates;

                foreach (Coordinate coordinate in coordinates)
                {
                    _selectionCoordinatesNet.Add(coordinate);
                }
            }
            
            _actionIsExecuting.Value = false;
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
            
            gameAction.Initialize(_executor);

            if (gameAction is INeedMap gameActionWithMap)
            {
                gameActionWithMap.Initialzie(_ship);
            }
            
            if (gameAction is INeedLootDeck needLootDeck)
            {
                while (_selectedLootTypesNet.Count != needLootDeck.RequiredLootDecksAmount)
                {
                    if (_actionIsExecuting.Value == false)
                    {
                        return;
                    }

                    await Awaitable.NextFrameAsync();
                }
                
                needLootDeck.LootDeckTypeSelection = _selectedLootTypesNet.ToEnumerable().Cast<LootDeck.Type>().ToArray();
            }
            
            if (gameAction is INeedInventoryItems gameActionWithInventoryItem)
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

            if (gameAction is INeedRooms gameActionWithRoomsSelection)
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

            if (gameAction is INeedRoomContents gameActionWithRoomContentSelection)
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

            if (gameAction is INeedCoordinates iNeedCoordinates)
            {
                while (iNeedCoordinates.RequiredCoordinatesAmount != _selectionCoordinatesNet.Count)
                {
                    if (_actionIsExecuting.Value == false)
                    {
                        return;
                    }

                    await Awaitable.NextFrameAsync();
                }

                iNeedCoordinates.CoordinatesSelection = _selectionCoordinatesNet.CashedElements.ToArray();
            }

            gameAction.Execute();
            
            if (gameAction is INeedPayment gameActionWithPayment)
            {
                while (_selectionActionCardsNet.Count != gameActionWithPayment.RequaredPaymentCount)
                {
                    if (_actionIsExecuting.Value == false)
                    {
                        return;
                    }
                    
                    await Awaitable.NextFrameAsync();
                }
                
                ActionCard[] cards = await _selectionActionCardsNet.GetElements();
                _executor.ActionCardsDeck.DiscardCards(cards);
            }
            
            ClearData_RPC();
        }

        [Rpc(SendTo.Owner)]
        private void ClearData_RPC()
        {
            _selectedLootTypesNet.Clear();
            _roomsSelectionNet.Clear();
            _selectionActionCardsNet.Clear();
            _noiseContainerSelectionNet.Clear();
            _roomContentSelectionNet.Clear();
            _inventoryItemsSelection.Clear();
            _inventoryItemsSelectionNet.Clear();
            _actionIsExecuting.Value = false;
        }
    }
}
