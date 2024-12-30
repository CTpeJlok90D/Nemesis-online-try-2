using UnityEngine;

namespace Unity.Netcode.Custom
{
    public class ServerObject : MonoBehaviour
    {
        private void Awake()
        {
            NetworkManager.Singleton.OnClientStarted += OnClientStart;
            NetworkManager.Singleton.OnClientStopped += OnClientStop;
            gameObject.SetActive(NetworkManager.Singleton.IsServer);
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientStarted -= OnClientStart;
                NetworkManager.Singleton.OnClientStopped -= OnClientStop;
            }
        }

        private void OnClientStart()
        {
            gameObject.SetActive(NetworkManager.Singleton.IsServer);
        }

        private void OnClientStop(bool obj)
        {
            gameObject.SetActive(NetworkManager.Singleton.IsServer);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gameObject.activeSelf == false)
            {
                gameObject.SetActive(true);
                Debug.LogWarning("Server object must be enebled on game start!");
            }

            if (enabled == false)
            {
                enabled = true;
                Debug.LogWarning("Server object must be enebled on game start!");
            }
        }
#endif
    }
}
