using System.Linq;
using Core.Aliens;
using Core.PlayerTablets;
using Unity.Netcode;

namespace Core.CharacterWeapons
{
    public class AttackMissSelfDamage : NetworkBehaviour, IAttackPostProcessor
    {
        private int heavyDamageMissCount = 1;

        public void PostProcessAttack(PlayerTablet playerTablet, Enemy target, AttackDice.Result rollResult)
        {
            if (target.AttacksToHit.Contains(rollResult))
            {
                return;
            }

            playerTablet.Health.HeavyDamage();
        }
    }
}
