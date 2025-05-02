using Unity.Netcode;

namespace Core.Aliens
{
    public class ImmediateDeath : NetworkBehaviour, IAlienDamageHandler
    {
        public void Handle(int damage, bool disableKilling = false)
        {
            if (disableKilling)
            {
                return;
            }

            if (damage == 0)
            {
                return;
            }
            
            ForceKill();
        }

        public void ForceKill()
        {
            NetworkObject.Despawn();
        }
    }
}
