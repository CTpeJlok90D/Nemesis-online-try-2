using System.Linq;
using UI.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UI.Selection.InventoryItemsSelections
{
    public class InventoryItemSelectionToggle : MonoBehaviour
    {
        [FormerlySerializedAs("_itemContainer")] [SerializeField] private InventoryItemSelectionItem _itemSelectionItem;
        [SerializeField] private PointerEvents _pointerEvents;
        
        private InventoryItemsSelectionItemsSpawner ItemsSpawner => _itemSelectionItem.Spawner;

        private void OnEnable()
        {
            _pointerEvents.PointerClicked += OnPointerClick;
        }

        private void OnDisable()
        {
            _pointerEvents.PointerClicked -= OnPointerClick;
        }

        private void OnPointerClick(PointerEvents sender, PointerEventData eventData)
        {
            if (ItemsSpawner.SelectedItems.Contains(_itemSelectionItem))
            {
                ItemsSpawner.DeselectItem(_itemSelectionItem);
            }
            else
            {
                ItemsSpawner.SelectItem(_itemSelectionItem);
            }
        }
    }
}
