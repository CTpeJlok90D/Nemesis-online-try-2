using Core.LootDecks;
using Core.SelectionBase;

namespace Core.Selection.LootDeckSelections
{
    public class LootDeckSelection : Selection<LootDeck.Type>
    {
        public override bool SkipSelectionWithSameItemCount => true;
    }
}
