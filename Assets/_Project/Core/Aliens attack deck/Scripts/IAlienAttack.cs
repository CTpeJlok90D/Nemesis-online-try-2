using Core.Aliens;
using Core.PlayerTablets;

namespace Core.AlienAttackDecks
{
    public interface IAlienAttack
    {
        public void Attack(Enemy executor, PlayerTablet target);
    }
}
