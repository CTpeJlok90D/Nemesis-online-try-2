using Core.Conncetion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UI.Loading;
using Zenject;
using System.Threading.Tasks;

namespace UI.Connection
{
    public class ConnectButton : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _input;
        [SerializeField] private Button _startClientButton;

        [Inject] private RelayConnection _relay;
        [Inject] private LoadScreen _loadScreen;

        private void OnEnable()
        {
            _startClientButton.onClick.AddListener(StartClient);
        }

        private void OnDisable()
        {
            _startClientButton.onClick.RemoveListener(StartClient);
        }
        
        private void StartClient()
        {
            Task task = _relay.JoinRelay(_input.text);
            _ = _loadScreen.StartLoading(task);
        }
    }
}
