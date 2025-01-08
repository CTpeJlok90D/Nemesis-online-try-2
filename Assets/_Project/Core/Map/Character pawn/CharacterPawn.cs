using Core.Characters;
using Unity.Netcode;
using UnityEngine;

namespace Core.Maps.CharacterPawns
{
    [RequireComponent(typeof(RoomContent))]
    [Icon("Assets/_Project/Core/Map/Editor/icons8-pawn-96.png")]
    public class CharacterPawn : NetworkBehaviour
    {
        [field: SerializeField, HideInInspector] public RoomContent RoomContent { get; private set; }
        [field: SerializeField] public Character LinkedCharacter { get; private set; }


#if UNITY_EDITOR
        private void OnValidate()
        {
            RoomContent = GetComponent<RoomContent>();
        }
#endif
    }
}
