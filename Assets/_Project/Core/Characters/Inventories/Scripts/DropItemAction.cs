using System;
using System.Collections.Generic;
using System.Linq;
using Core.Maps;
using Core.PlayerActions;
using Core.PlayerActions.Base;
using Core.PlayerTablets;
using UnityEngine;

namespace Core.CharacterInventories
{
    [CreateAssetMenu(menuName = CreateAssetMenuPaths.Actions + "Drop item action")]
    public class DropItemAction : ScriptableObject, IGameAction, INeedInventoryItems
    {
        [SerializeField] private DroppedInRoomItem _droppedItem_PREFAB; 
        
        private PlayerTablet _executor;
        public int RequiredItemsAmount => 1;

        public InventoryItem[] InventoryItemsSource
        {
            get
            {
                List<InventoryItem> result = new();
                result.AddRange(_executor.BigItemsInventory.Items);
                result.AddRange(_executor.SmallItemsInventory.Items);
                return result.ToArray();
            }
        }

        public InventoryItem[] InventoryItemsSelection { get; set; } = Array.Empty<InventoryItem>();
        
        public void Initialize(PlayerTablet executor)
        {
            _executor = executor;
        }

        public IGameAction.CanExecuteCheckResult CanExecute()
        {
            try
            {
                if (_executor.BigItemsInventory.Any() == false && _executor.SmallItemsInventory.Any() == false)
                {
                    return new IGameAction.CanExecuteCheckResult()
                    {
                        Result = false,
                        Error = new Exception("Items not found"),
                    };
                }

                return new IGameAction.CanExecuteCheckResult()
                {
                    Result = true,
                };
            }
            catch (Exception e)
            {
                return new IGameAction.CanExecuteCheckResult()
                {
                    Result = false,
                    Error = e
                };
            }
        }

        public void ForceExecute()
        {
            RoomCell roomCell = _executor.CharacterPawn.RoomContent.Owner;
            InventoryItem itemToDrop = InventoryItemsSelection.First();
            
            _executor.RemoveItem(itemToDrop);
            _droppedItem_PREFAB.Instantiate(roomCell, itemToDrop);
        }
    }
}

