using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Core.CharacterInventorys;
using UnityEngine;

namespace Core.CharacterWeapons
{
    [Icon("Assets/_Project/Core/Character weapons/Editor/icons8-weapon-96.png")]
    [CreateAssetMenu(menuName = "Game/Inventory/Magazine size")]
    public class MagazineSize : ScriptableObject, IItemData
    {
        public const string MAGAZINE_SIZE_DATA = "Magazine size";
        public const string START_AMMO_COUNT = "Start ammo count";
        
        [field: SerializeField] public int Size { get; private set; }
        [field: SerializeField] public int AmmoCount { get; private set; } = 1;
        
        SerializedDictionary<string, string> IItemData.StartItemData
        {
            get
            {
                SerializedDictionary<string, string> result = new()
                {
                    { START_AMMO_COUNT, AmmoCount.ToString() },
                    { MAGAZINE_SIZE_DATA, Size.ToString() }
                };

                return result;
            }
        }
    }
}