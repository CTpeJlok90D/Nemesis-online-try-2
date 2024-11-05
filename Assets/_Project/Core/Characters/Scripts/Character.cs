using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.Characters
{
    [Icon("Assets/_Project/Core/Characters/Editor/icons8-person-96.png")]
    [CreateAssetMenu(menuName = "Game/Character")]
    public class Character : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject CharacterPawn { get; private set; }
        [field: SerializeField] public AssetReferenceT<Sprite> CharacterAvatar { get; private set; }
        [field: SerializeField] public AssetReferenceT<Sprite> CharacterBackground { get; private set; }
    }   
}