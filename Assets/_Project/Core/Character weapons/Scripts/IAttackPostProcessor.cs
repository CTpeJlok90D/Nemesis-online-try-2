using Core.Aliens;
using Core.PlayerTablets;
using UnityEngine;

namespace Core.CharacterWeapons
{
    public interface IAttackPostProcessor
    {
        public void PostProcessAttack(PlayerTablet playerTablet, Enemy target, AttackDice.Result rollResult);
    }
}
