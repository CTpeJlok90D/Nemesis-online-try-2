using System.Collections.Generic;
using Core.CharacterInventorys;
using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace Core.CharacterWeapons
{
    [Icon("Assets/_Project/Core/Character weapons/Editor/icons8-hate-96.png")]
    [CreateAssetMenu(menuName = "Game/Inventory/Weapon damage")]
    public class WeaponDamage : ScriptableObject, IItemData
    {
        public const string DAMAGE_DATA_NAME = "Damage";
        public const string CONST_DAMAGE_DATA_NAME = "Const damage";
        public const string ADDITIONAL_DAMAGE_DATA_NAME = "Additional damage";
        
        [SerializeField] private SerializedDictionary<AttackDice.Result, int> _additionalDamagePerAttackTypes;
        [SerializeField] private int _baseDamage = 1;
        [SerializeField] private int _constantDamage = 0;

        public IReadOnlyDictionary<AttackDice.Result, int> AdditionalDamagePerAttackTypes => _additionalDamagePerAttackTypes;
        public int BaseDamage => _baseDamage;
        public int ConstantDamage => _constantDamage;

        SerializedDictionary<string, string> IItemData.StartItemData
        {
            get
            {
                SerializedDictionary<string, string> result = new()
                {
                    { DAMAGE_DATA_NAME, _baseDamage.ToString() },
                    { CONST_DAMAGE_DATA_NAME, _constantDamage.ToString() },
                    { ADDITIONAL_DAMAGE_DATA_NAME, JsonUtility.ToJson(_additionalDamagePerAttackTypes) }
                };

                return result;
            }
        }
    }
}