using System.Collections.Generic;
using System.Linq;
using Core.CharacterInventories;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using Core.Aliens;
using Unity.Netcode;

namespace Core.CharacterWeapons
{
    [Icon("Assets/_Project/Core/Character weapons/Editor/icons8-hate-96.png")]
    [CreateAssetMenu(menuName = "Game/Inventory/Weapon damage")]
    public class WeaponDamage : NetworkBehaviour
    {
        [SerializeField] private SerializedDictionary<AttackDice.Result, int> _additionalDamagePerAttackTypes;
        [SerializeField] private int _baseDamage = 1;
        [SerializeField] private int _constantDamage = 0;

        public IReadOnlyDictionary<AttackDice.Result, int> AdditionalDamagePerAttackTypes => _additionalDamagePerAttackTypes;
        public int BaseDamage => _baseDamage;
        public int ConstantDamage => _constantDamage;

        public int GetDamageFor(AttackDice.Result rollResult, Enemy enemy)
        {
            int result = _constantDamage;

            if (enemy.AttacksToHit.Contains(rollResult))
            {
                result += _baseDamage;
                if (AdditionalDamagePerAttackTypes.TryGetValue(rollResult, out int value))
                {
                    result += value;
                }
            }
            
            return result;
        }
    }
}