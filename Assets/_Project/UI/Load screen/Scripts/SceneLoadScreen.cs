using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace UI.Loading
{
    [DefaultExecutionOrder(1)]
    public class SceneLoadScreen : MonoBehaviour
    {
        [Inject] private LoadScreen _loadScreen;
        private NetworkManager NetworkManager => NetworkManager.Singleton;

        private void OnEnable()
        {
            NetworkManager.OnClientStarted += OnClientStart;
        }

        private void OnDisable()
        {
            if (NetworkManager != null)
            {
                NetworkManager.OnClientStarted -= OnClientStart;
            }
        }

        private void OnClientStart()
        {
            NetworkManager.SceneManager.OnLoad += OnSceneEvent;
        }

        private void OnSceneEvent(ulong clientId, string sceneName, LoadSceneMode loadSceneMode, AsyncOperation asyncOperation)
        {
            _ = _loadScreen.Show(SceneLoadTask(asyncOperation));   
        }

        private async Task SceneLoadTask(AsyncOperation asyncOperation)
        {
            try
            {
                while (asyncOperation.isDone == false)
                {
                    await Awaitable.NextFrameAsync();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
