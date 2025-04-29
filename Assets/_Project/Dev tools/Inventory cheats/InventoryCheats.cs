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
            PlayerTablet tablet = _playerTabletList.ActiveTablets.FirstOrDefault(x => x.Player.GetComponent<NicknameContainer>().Value == playerName);

            if (tablet == null)
            {
                Debug.LogError($"Player {playerName} not found");
                return;
            }
            
            AsyncOperationHandle<InventoryItem> handle = Addressables.LoadAssetAsync<InventoryItem>(item);
            await handle.ToUniTask();
            
            InventoryItem inventoryItem = handle.Result;
            if (inventoryItem.ItemType == ItemType.Big)
            {
                tablet.BigItemsInventory.AddItem(inventoryItem);
            }

            if (inventoryItem.ItemType == ItemType.Small)
            {
                tablet.SmallItemsInventory.AddItem(inventoryItem);
            }
            
            Debug.Log($"Item {inventoryItem.ID} was given to {playerName}");
        }
    }
}