using System.Collections.Generic;
using System.Linq;
using Core.CharacterInventorys;
using Core.Players;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using IngameDebugConsole;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Devtools.Maps
{
    public class InventoryCheats : MonoBehaviour
    {
        [Inject] private PlayerTabletList _playerTabletList;
        private void Awake()
        {
            DebugLogConsole.AddCommand<string, string>("Give", "Give item to player. ItemID, PlayerNickname", GiveItem);
        }

        private void GiveItem(string itemID, string playerNickname) => _ = GiveItemAsync(itemID, playerNickname);
        
        private async UniTask GiveItemAsync(string item, string playerName)
        {
            Target target = new(playerName, _playerTabletList);
            
            AsyncOperationHandle<InventoryItem> handle = Addressables.LoadAssetAsync<InventoryItem>(item);
            await handle.ToUniTask();
            
            InventoryItem inventoryItem = handle.Result;

            foreach (PlayerTablet playerTablet in target)
            {
                if (inventoryItem.ItemType == ItemType.Big)
                {
                    playerTablet.BigItemsInventory.AddItem(inventoryItem);
                }

                if (inventoryItem.ItemType == ItemType.Small)
                {
                    playerTablet.SmallItemsInventory.AddItem(inventoryItem);
                }
            }
            
            Debug.Log($"Item {inventoryItem.ID} was given to {target}");
        }
    }
}