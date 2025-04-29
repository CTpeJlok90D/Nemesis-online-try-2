using Core.Aliens;
using Core.Maps.CharacterPawns;

namespace Core.AlienAttackDecks
{
    public interface IAlienAttack
    {
        public void Attack(Enemy executor, CharacterPawn target);
    }
}
