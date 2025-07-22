using System.Linq;
using Core.PlayerTablets;
using Core.Starter;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Core.ResultEvaluators
{
    public class ResultEvaluator : MonoBehaviour
    {
        [Inject] private Activator _activator;

        private void OnEnable()
        {
            PlayerTablet.Despawned += OnTabletRemove;
        }

        private void OnDisable()
        {
            PlayerTablet.Despawned -= OnTabletRemove;
        }

        private void OnTabletRemove(PlayerTablet spawned)
        {
            if (NetworkManager.Singleton == null || NetworkManager.Singleton.IsServer == false)
            {
                return;
            }
            
            if (PlayerTablet.Instances.Any() == false)
            {
                _ = _activator.StopGame();
            }
        }
    }
}
