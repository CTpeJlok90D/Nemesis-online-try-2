using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace UI.Loading
{
    public class SceneLoadScreen : MonoBehaviour
    {
        [Inject] private LoadScreen _loadScreen;
        [Inject] private NetworkManager _networkManager;

        private void OnEnable()
        {
            _networkManager.OnClientStarted += OnClientStart;
        }

        private void OnDisable()
        {
            _networkManager.OnClientStarted -= OnClientStart;
        }

        private void OnClientStart()
        {
            _networkManager.SceneManager.OnLoad += OnSceneEvent;
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
