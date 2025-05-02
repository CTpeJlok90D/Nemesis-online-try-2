using UnityEngine;

namespace Core.Aliens
{
    public interface IAlienDamageHandler
    {
        public void Handle(int damage, bool disableKilling = false);
        public void ForceKill();
    }
}
