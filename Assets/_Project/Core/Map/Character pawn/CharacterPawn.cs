using Core.ActionsCards;
using Core.CharacterInventorys;
using Core.Characters;
using Core.Characters.Health;
using Unity.Netcode;
using UnityEngine;

namespace Core.Maps.CharacterPawns
{
    [RequireComponent(typeof(RoomContent))]
    [Icon("Assets/_Project/Core/Map/Editor/icons8-pawn-96.png")]
    public class CharacterPawn : NetworkBehaviour
    {
        [field: SerializeField] public Inventory SmallItemsInventory { get; private set; }
        [field: SerializeField] public Inventory BigItemsInventory { get; private set; }
        [field: SerializeField] public ActionCardsDeck ActionCardsDeck { get; private set; }
        [field: SerializeField] public CharacterHealth Health { get; private set; }
        [field: SerializeField] public Character LinkedCharacter { get; private set; }
        
        [field: SerializeField, HideInInspector] public RoomContent RoomContent { get; private set; }

        public CharacterPawn Instantiate()
        {
            gameObject.SetActive(false);
            CharacterPawn characterInstance = Object.Instantiate(this);
            gameObject.SetActive(true);
                
            characterInstance.gameObject.SetActive(true);
            characterInstance.NetworkObject.Spawn();
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
