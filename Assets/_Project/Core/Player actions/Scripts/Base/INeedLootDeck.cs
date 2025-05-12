using Core.CharacterInventories;
using Core.LootDecks;
using Core.Selection.LootDeckSelections;
using Cysharp.Threading.Tasks;

namespace Core.PlayerActions
{
    public interface INeedLootDeck 
    {
        public int RequiredLootDecksAmount { get; }
        
        public LootDeck.Type[] LootDecksSource { get; }
        
        public LootDeck.Type[] InventoryItemsSelection { get; set; }
        
        internal async UniTask<LootDeck.Type[]> GetSelectionLocal(LootDeckSelection selection)
        {
            LootDeck.Type[] selected = await selection.SelectFrom(LootDecksSource, RequiredLootDecksAmount);

            InventoryItemsSelection = selected;
            return selected;
        }
    }
}
