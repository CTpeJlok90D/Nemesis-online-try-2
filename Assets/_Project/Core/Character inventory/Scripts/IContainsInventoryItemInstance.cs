namespace Core.CharacterInventorys
{
    public interface IContainsInventoryItemInstance
    {
        public InventoryItemInstance Item { get; }
        
        public string ID => Item.ID;
    }
}
