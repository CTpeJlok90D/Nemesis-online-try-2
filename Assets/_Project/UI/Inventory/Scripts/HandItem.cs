using System.Linq;
using Core.CharacterInventories;
using Core.PlayerTablets;
using UnityEngine;

namespace UI
{
    public class HandItem : MonoBehaviour, IContainsInventoryItemInstance
    {
        [field: SerializeField] public int ItemIndex { get; private set; }

        public InventoryItem Item => PlayerTablet.Local?.CharacterPawn?.BigItemsInventory?.Items?.ElementAtOrDefault(ItemIndex);
    }
}