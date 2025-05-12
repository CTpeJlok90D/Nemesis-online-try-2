using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.CharacterInventories;
using Core.Entities;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using Random = UnityEngine.Random;

namespace Core.LootDecks
{
    public class LootDeck : NetEntity<LootDeck>
    {
        [SerializeField] private Type _type;
        [Inject] private LootDecksConfig _config;

        private NetworkList<NetworkObjectReference> _items;

        public Type DeckType => _type;

        private void Awake()
        {
            _items = new();
        }

        private void Start()
        {
            if (NetworkManager.IsServer)
            {
                _ = InitializeDeck();
            }
        }

        private async UniTask InitializeDeck()
        {
            List<InventoryItem> itemInstances = new();
            foreach (AssetReference item in _config.DeckItems[_type])
            {
                GameObject itemInstanceObject = await item.InstantiateAsync().ToUniTask();
                InventoryItem itemInstance = itemInstanceObject.GetComponent<InventoryItem>();
                itemInstance.NetworkObject.Spawn();
                itemInstance.NetworkObject.TrySetParent(NetworkObject);
                itemInstances.Add(itemInstance);
            }
            itemInstances = itemInstances.OrderBy(x => Random.value).ToList();
            _items.Clear();
            foreach (InventoryItem item in itemInstances)
            {
                _items.Add(item.NetworkObject);
            }
        }

        public void Shuffle()
        {
            NetworkObjectReference[] shuffledDeck = _items.ToEnumerable().OrderBy(x => Random.value).ToArray();
            _items.Clear();
            foreach (NetworkObjectReference item in shuffledDeck)
            {
                _items.Add(item);
            }
        }

        public InventoryItem[] GetItems(int count)
        {
            InventoryItem[] items = _items.ToEnumerable<InventoryItem>().Take(count).ToArray();
            return items;
        }

        public void RemoveItems(params InventoryItem[] items)
        {
            foreach (InventoryItem item in items)
            {
                _items.Remove(item.NetworkObject);
                item.NetworkObject.TryRemoveParent();
            }
        }
        
        public void AddItems(params InventoryItem[] items)
        {
            foreach (InventoryItem item in items)
            {
                _items.Add(item.NetworkObject);
                item.NetworkObject.TrySetParent(NetworkObject);
            }
        }
        
        public enum Type
        {
            MedDeck,
            BattleDeck,
            TechDeck,
        }
    }
}