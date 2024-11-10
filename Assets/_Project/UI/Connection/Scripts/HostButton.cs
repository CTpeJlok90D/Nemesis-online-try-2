using Core.Conncetion;
using System.Threading.Tasks;
using UI.Loading;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Starter
{
    public class HostButton : MonoBehaviour
    {
        
        [SerializeField] private Button _startHostButton;

        [Inject] private RelayConnection _relay;
        [Inject] private LoadScreen _loadScreen;

        private void OnEnable()
        {
            _startHostButton.onClick.AddListener(StartHost);
        }

        private void OnDisable()
        {
            _startHostButton.onClick.RemoveListener(StartHost);
        }

        private void StartHost()
        {
            Task task = _relay.CreateRelay();
            _ = _loadScreen.Show(task);
        }
    }
}
