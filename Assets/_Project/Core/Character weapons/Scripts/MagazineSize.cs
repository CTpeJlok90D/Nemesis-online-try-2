using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.CharacterWeapons
{
    [Icon("Assets/_Project/Core/Character weapons/Editor/icons8-weapon-96.png")]
    [CreateAssetMenu(menuName = "Game/Inventory/Magazine size")]
    public class MagazineSize : NetworkBehaviour
    {
        [field: SerializeField] public int Size { get; private set; }
        [field: SerializeField] public NetVariable<int> AmmoCount { get; private set; }
    }
}