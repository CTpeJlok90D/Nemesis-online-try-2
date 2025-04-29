using UnityEngine;

namespace Core.Aliens
{
    public interface IAlienDamageHandler
    {
        public void Handle(Enemy target, int damage, bool disableKilling = false);
    }
}
