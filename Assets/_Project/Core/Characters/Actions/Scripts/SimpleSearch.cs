using System;
using System.Linq;
using Core.ActionsCards;
using Core.CharacterInventories;
using Core.LootDecks;
using Core.Maps;
using Core.PlayerActions;
using Core.PlayerActions.Base;
using Core.PlayerTablets;
using UnityEngine;

namespace Core.Characters.Actions
{
    [CreateAssetMenu(menuName = Constants.ACTIONS_CREATE_PARH + "Simple search")]
    public class SimpleSearch : ScriptableObject, IGameAction, INeedInventoryItems, INeedLootDeck
    {
        public const string SearchCardId = "Search";
        
        public const int SelectItemsFrom = 2;
        public int RequiredItemsAmount => 1;
        public int RequiredLootDecksAmount => 1;
        public PlayerTablet Executor { get; private set; }
        public InventoryItem[] InventoryItemsSelection { get; set; } = Array.Empty<InventoryItem>();
        LootDeck.Type[] INeedLootDeck.InventoryItemsSelection { get; set; } = Array.Empty<LootDeck.Type>();
        public bool CanCancel => false;

        public LootDeck.Type[] LootDecksSource
        {
            get
            {
                RoomContent roomContent = Executor.CharacterPawn.RoomContent;
                RoomCell roomCell = roomContent.Owner;
                Debug.Log(roomCell, roomCell);
                
                switch (roomCell.Loot)
                {
                    case RoomType.LootType.UniversalRoom:
                        return new[]
                        {
                            LootDeck.Type.BattleDeck,
                            LootDeck.Type.MedDeck,
                            LootDeck.Type.TechDeck,
                        };
                    case RoomType.LootType.BattleRoom:
                        return new [] { LootDeck.Type.BattleDeck} ;
                    case RoomType.LootType.MedicineRoom:
                        return new [] { LootDeck.Type.MedDeck} ;
                    case RoomType.LootType.TechnicalRoom:
                        return new [] { LootDeck.Type.TechDeck} ;
                }
                
                throw new InvalidOperationException($"Deck for room with type {roomCell.Loot} was not found.");
            }
        }

        public InventoryItem[] InventoryItemsSource
        {
            get
            {
                InventoryItem[] itemsToSelectFrom = LootDeck.GetItems(SelectItemsFrom);
                Debug.Log($"Loot deck type = {string.Join(", ", LootDecksSource)}", LootDeck);
                Debug.Log(string.Join(", ", itemsToSelectFrom.Select(x => x.ID)), LootDeck);
                return itemsToSelectFrom;
            }
        }

        private LootDeck LootDeck
        {
            get { return LootDeck.Instances.First(x => x.DeckType == LootDecksSource.First()); }
        }

        public void Initialize(PlayerTablet executor)
        {
            Executor = executor;
        }

        public static bool RoomIsValidToLoot(RoomCell roomCell)
        {
            return roomCell.LootCount.Value != 0;
        }

        public static bool ExecutorHaveCard(PlayerTablet playerTablet)
        {
            return playerTablet.ActionCardsDeck.HandLocal.Any(x => x.ID == SearchCardId);
        }

        public IGameAction.CanExecuteCheckResult CanExecute()
        {
            RoomCell roomCell = OwnerRoomCell;

            if (RoomIsValidToLoot(roomCell) == false)
            {
                return new IGameAction.CanExecuteCheckResult()
                {
                    Result = false,
                    Error = new InvalidOperationException("Loot count must be greater than 0"),
                };
            }

            if (ExecutorHaveCard(Executor) == false)
            {
                return new IGameAction.CanExecuteCheckResult()
                {
                    Result = false,
                    Error = new InvalidOperationException("Executor don't have search card"),
                };
            }

            return new IGameAction.CanExecuteCheckResult()
            {
                Result = true
            };
        }

        private RoomCell OwnerRoomCell
        {
            get
            {
                RoomContent roomContent = Executor.CharacterPawn.RoomContent;
                RoomCell roomCell = roomContent.Owner;
                return roomCell;
            }
        }

        public void Execute()
        {
            if (CanExecute() == false)
            {
                throw CanExecute().Error;
            }
            
            ForceExecute();
        }

        public void ForceExecute()
        {
            InventoryItem[] source = InventoryItemsSource;
            InventoryItem selectedItem = InventoryItemsSelection.First();
            
            LootDeck.RemoveItems(source);
            LootDeck.AddItems(source.First(x => x != selectedItem));
            Executor.AddItem(selectedItem);
            
            OwnerRoomCell.LootCount.Value--;
            Executor.ActionCount.Value--;

            ActionCard card = Executor.ActionCardsDeck.HandLocal.First(x => x.ID == SearchCardId);
            Executor.ActionCardsDeck.DiscardCard(card);
        }
    }
}
