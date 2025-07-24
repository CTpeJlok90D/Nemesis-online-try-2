using System.Linq;
using Core.Maps.CharacterPawns;
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
            CharacterPawn.Despawned += OnTabletRemove;
        }

        private void OnDisable()
        {
            CharacterPawn.Despawned -= OnTabletRemove;
        }

        private void OnTabletRemove(CharacterPawn spawned)
        {
            if (NetworkManager.Singleton == null || NetworkManager.Singleton.IsServer == false)
            {
                return;
            }
            
            if (CharacterPawn.Instances.Any() == false)
            {
                _ = _activator.StopGame();
            }
        }
    }
}
