using Core.Entities;
using Core.Maps;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.CharacterInventories
{
    [RequireComponent(typeof(RoomContent))]
    public class DroppedInRoomItem : NetEntity<DroppedInRoomItem>
    {
        public RoomContent RoomContent { get; private set; }
        public NetBehaviourReference<InventoryItem> Item { get; private set; }
        
        public DroppedInRoomItem Instantiate(RoomCell cell, InventoryItem item)
        {
            gameObject.SetActive(false);
            DroppedInRoomItem result = Instantiate(this);
            gameObject.SetActive(true);

            result.RoomContent = result.GetComponent<RoomContent>();
            result.Item = new(item);
            result.gameObject.SetActive(true);
            result.NetworkObject.Spawn();
            
            cell.AddContent(result.RoomContent);
            item.NetworkObject.TrySetParent(result.NetworkObject);
            
            return result;
        }

        private void Awake()
        {
            Item ??= new();
        }
    }
}