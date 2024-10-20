using UnityEngine;

namespace Unity.Netcode.Custom
{
    public class OwnershipObject : NetworkBehaviour
    {
        [SerializeField] private GameObject[] _targets;
        [SerializeField] private bool _inverse;
        
        public override void OnGainedOwnership()
        {
            ValidateTargetsActiveState();
        }

        public override void OnNetworkSpawn()
        {
            ValidateTargetsActiveState();
        }

        public override void OnLostOwnership()
        {
            ValidateTargetsActiveState();
        }

        private void ValidateTargetsActiveState()
        {
            bool localClientIsOwner = NetworkManager.Singleton.LocalClientId == NetworkObject.OwnerClientId;
            if (_inverse)
            {
                localClientIsOwner = localClientIsOwner == false;
            }

            foreach (GameObject target in _targets) 
            {
                target.SetActive(localClientIsOwner);   
            }
        }
    }
}
