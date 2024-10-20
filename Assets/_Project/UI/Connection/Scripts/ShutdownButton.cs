using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Connection
{
    public class ShutdownButton : MonoBehaviour
    {
        [SerializeField] private Button _stopButton;
        
        private void OnEnable()
        {
            _stopButton.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _stopButton.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}
