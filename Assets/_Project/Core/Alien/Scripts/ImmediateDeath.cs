using Unity.Netcode;

namespace Core.Aliens
{
    public class ImmediateDeath : NetworkBehaviour, IAlienDamageHandler
    {
        public void Handle(Enemy target, int damage, bool disableKilling = false)
        {
            if (disableKilling)
            {
                return;
            }
            
            NetworkObject.Despawn();
        }
    }
}
