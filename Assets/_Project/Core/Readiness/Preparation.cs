using Unity.Netcode;
using Unity.Netcode.Custom;

namespace Core.Readiness
{
    public class Preparation : NetworkBehaviour
    {
        public NetVariable<bool> IsReady { get; private set; }

        private void Awake()
        {
            IsReady = new(false, writePerm: NetworkVariableWritePermission.Owner);
        }
    }
}