using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Core.Starter
{
    public class Activator
    {
        private string _activatedGameSceneName;
        private float _loadDelay = 0.5f;

        public Activator(string activeGameSceneName)
        {
            _activatedGameSceneName = activeGameSceneName;
        }

        public async Task StartGame()
        {
            try
            {
                if (NetworkManager.Singleton.IsServer == false)
                {
                    throw new NotServerException("Only server can start game");
                }
                
                await Awaitable.WaitForSecondsAsync(_loadDelay);
                
                NetworkManager.Singleton.SceneManager.LoadScene(_activatedGameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
                bool isCompleted = false;
                NetworkManager.Singleton.SceneManager.OnLoadComplete += (clientID, sceneName, mode) => 
                {
                    isCompleted = true;
                };


                while (isCompleted == false)
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
