namespace Core.CharacterInventories
{
    public interface IContainsInventoryItemInstance
    {
        public InventoryItem Item { get; }
        
        public string ID => Item.ID;
    }
}
