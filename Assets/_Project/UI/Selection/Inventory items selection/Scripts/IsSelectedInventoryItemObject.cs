using System.Linq;
using Core.Selection.InventoryItems;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace UI.Selection.InventoryItemsSelections
{
    public class IsSelectedInventoryItemObject : MonoBehaviour
    {
        [FormerlySerializedAs("_itemContainer")] [SerializeField] private InventoryItemSelectionItem _itemSelectionItem;
        [SerializeField] private GameObject _target;
        
        [Inject] private InventoryItemsSelection _selection;
        
        private InventoryItemsSelectionItemsSpawner ItemsSpawner => _itemSelectionItem.Spawner;  

        private void Update()
        {
            _target.SetActive(ItemsSpawner.SelectedItems.Contains(_itemSelectionItem));
        }
    }
}
