using System;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Core.Starter
{
    public class Activator
    {
        private readonly string _activatedGameSceneName;
        private readonly string _lobbySceneName;
        private readonly float _loadDelay = 0.5f;

        private bool _gameIsActive;

        public delegate void ActivatedListener();
        public event ActivatedListener GameActivated;
        public delegate void DeactivatedListener();
        public event DeactivatedListener GameDeactivated;

        public Activator(string activeGameSceneName, string lobbySceneName)
        {
            _activatedGameSceneName = activeGameSceneName;
            _lobbySceneName = lobbySceneName;
        }

        public async UniTask StartGame()
        {
            if (NetworkManager.Singleton.IsServer == false)
            {
                throw new NotServerException("Only server can start game");
            }

            if (_gameIsActive)
            {
                throw new InvalidOperationException("Game is already active");
            }
                
            await Awaitable.WaitForSecondsAsync(_loadDelay);
            GameActivated?.Invoke();
                
            
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

            _gameIsActive = true;
        }

        public async UniTask StopGame()
        {
            if (NetworkManager.Singleton.IsServer == false)
            {
                throw new NotServerException("Only server can stop game");
            }
            
            if (_gameIsActive == false)
            {
                throw new InvalidOperationException("Game is already inactive");
            }
            
            await Awaitable.WaitForSecondsAsync(_loadDelay);
            GameDeactivated?.Invoke();
            
            NetworkManager.Singleton.SceneManager.LoadScene(_lobbySceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
            bool isCompleted = false;
            NetworkManager.Singleton.SceneManager.OnLoadComplete += (clientID, sceneName, mode) => 
            {
                isCompleted = true;
            };
            
            while (isCompleted == false)
            {
                await Awaitable.NextFrameAsync();
            }
            
            _gameIsActive = false;
        }
    }
}
