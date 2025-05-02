using System;
using Core.ActionsCards;
using Core.CharacterInventories;
using Core.Characters;
using Core.Characters.Health;
using Core.Characters.Tokens;
using Core.Entity;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Maps.CharacterPawns
{
    [RequireComponent(typeof(RoomContent))]
    [Icon("Assets/_Project/Core/Map/Editor/icons8-pawn-96.png")]
    public class CharacterPawn : NetEntity<CharacterPawn>
    {
        [field: SerializeField] public Inventory SmallItemsInventory { get; private set; }
        [field: SerializeField] public Inventory BigItemsInventory { get; private set; }
        [field: SerializeField] public ActionCardsDeck ActionCardsDeck { get; private set; }
        [field: SerializeField] public CharacterHealth Health { get; private set; }
        [field: SerializeField] public Character LinkedCharacter { get; private set; }
        [field: SerializeField] private InventoryItem _hands_PREFAB;
        [field: SerializeField, HideInInspector] public RoomContent RoomContent { get; private set; }
        
        public NetScriptableObjectList4096<CharacterToken> Tokens { get; private set; }
        
        public static InventoryItem Hands { get; private set; }

        public CharacterPawn Instantiate()
        {
            gameObject.SetActive(false);
            CharacterPawn characterInstance = Object.Instantiate(this);
            gameObject.SetActive(true);
                
            characterInstance.Tokens = new NetScriptableObjectList4096<CharacterToken>();
            
            characterInstance.gameObject.SetActive(true);
            characterInstance.NetworkObject.Spawn();
            
            if (Hands == null)
            {
                Hands = Instantiate(_hands_PREFAB);
                Hands.NetworkObject.Spawn();
            }
            
            return characterInstance;
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            RoomContent = GetComponent<RoomContent>();
        }
#endif
    }
}
