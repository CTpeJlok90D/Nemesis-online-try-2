using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Core.GameInitialization
{
    public class Initializator : MonoBehaviour
    {
        [SerializeField] private string _mainMenuScene;
        [Inject] private InitializationProcessList _processList;

        private List<InitializationProcess> _processesToInit = new();

        private void Awake()
        {
            _processesToInit = _processList.ToList();
        }

        private void OnEnable()
        {
            foreach (InitializationProcess process in _processList)
            {
                process.Completed += OnProcessComplete;
            }
        }

        private void Start()
        {
            if (_processesToInit.Count == 0)
            {
                OnInitializationComplete();
            }
        }

        private void OnDisable()
        {
            foreach (InitializationProcess process in _processList)
            {
                process.Completed -= OnProcessComplete;
            }
        }

        private void OnProcessComplete(InitializationProcess sender)
        {
            _processesToInit.Remove(sender);

            if (_processesToInit.Count == 0)
            {
                OnInitializationComplete();
            }
        }

        private void OnInitializationComplete()
        {
            SceneManager.LoadSceneAsync(_mainMenuScene, LoadSceneMode.Single);
        }
    }
}
