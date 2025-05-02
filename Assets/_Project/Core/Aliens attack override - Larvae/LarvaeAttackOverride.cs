using Core.Aliens;
using Core.Characters.Tokens;
using Core.PlayerTablets;
using TNRD;
using UnityEngine;

namespace Core.AliensAttack.Larvae
{
    public class LarvaeAttackOverride : MonoBehaviour, IAlienAttackOverride
    {
        [SerializeField] private Enemy _enemy;
        [SerializeField] private SerializableInterface<IAlienDamageHandler> _damageHandler;
        [SerializeField] private CharacterToken _larvaeToken;

        public void Attack(Enemy executor, PlayerTablet target)
        {
            target.CharacterPawn.Tokens.Add(_larvaeToken);
            _damageHandler.Value.ForceKill();
        }
    }
}
