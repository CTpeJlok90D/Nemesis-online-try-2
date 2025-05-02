using Core.PlayerTablets;
using UnityEngine;

namespace Core.Aliens
{
    public interface IAlienAttackOverride
    {
        public void Attack(Enemy executor, PlayerTablet target);
    }
}
