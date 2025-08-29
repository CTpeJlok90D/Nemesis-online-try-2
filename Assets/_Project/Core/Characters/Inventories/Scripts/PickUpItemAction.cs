using System;
using System.Collections.Generic;
using System.Linq;
using Core.Maps;
using Core.Maps.CharacterPawns;
using Core.PlayerActions;
using Core.PlayerActions.Base;
using Core.PlayerTablets;
using UnityEngine;

namespace Core.CharacterInventories
{
    [CreateAssetMenu(menuName = CreateAssetMenuPaths.Actions + "Pick up item action")]
    public class PickUpItemAction : ScriptableObject, IGameAction, INeedPayment, INeedInventoryItems
    {
        private PlayerTablet _executor;
        public int RequaredPaymentCount => 1;
        public int RequiredItemsAmount => 1;

        private CharacterPawn CharacterPawn => _executor.CharacterPawn;
        private RoomCell OwnerRoomCell => CharacterPawn.RoomContent.Owner;
        public InventoryItem[] InventoryItemsSource
        {
            get
            {
                List<InventoryItem> result = new();
                
                RoomCell cell = _executor.CharacterPawn.RoomContent.Owner;
                foreach (RoomContent content in cell.RoomContents)
                {
                    if (content.TryGetComponent(out DroppedInRoomItem item))
                    {
                        result.Add(item.Item.Value);
                    }
                }

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
            bool founded = _executor.ActionCount.Value > 0 && 
                           _executor != null && 
                           CharacterPawn != null && 
                           OwnerRoomCell.RoomContents.Any(x => x.TryGetComponent(out DroppedInRoomItem item));

            IGameAction.CanExecuteCheckResult result = new()
            {
                Result = founded
            };
            
            if (founded == false)
            {
                result.Error = new Exception("Room have no dropped items or player have no action points");
            }

            return result;
        }

        public void ForceExecute()
        {
            InventoryItem item = InventoryItemsSelection.First();
            DroppedInRoomItem droppedItem = null;
            
            OwnerRoomCell.RoomContents.First(x => x.TryGetComponent(out droppedItem));
            droppedItem.NetworkObject.Despawn();
            
            _executor.AddItem(item);
            _executor.ActionCount.Value--;
        }
    }
}