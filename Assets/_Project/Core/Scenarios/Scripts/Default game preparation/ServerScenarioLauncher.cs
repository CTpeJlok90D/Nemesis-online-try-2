using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Core.Scenarios.Default
{
    public class ServerScenarioLauncher : MonoBehaviour
    {
        [SerializeField] private ScenarioLauncher _scenarioLauncher;

        [Inject] private NetworkManager _networkManager;

        private void Start()
        {
            if (_networkManager.IsServer)
            {
                _scenarioLauncher.Launch();
            }
        }
    }
}
