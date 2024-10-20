using Core.Players;
using Core.Readiness;
using System;
using UI.CommonScripts;
using Unity.Netcode;
using UnityEngine;

namespace UI.Readiness
{
    public class ReadyTabs : MonoBehaviour
    {
        [SerializeField] private Tab _readyTab;
        [SerializeField] private Tab _notReadyTab;

        private Preparation _localPlayerPreparation;

        private void OnEnable()
        {
            NetworkManager.Singleton.OnClientStarted += OnClientStart;
        }

        private void OnDisable()
        {
            NetworkManager.Singleton.OnClientStarted -= OnClientStart;
        }

        private void OnClientStart()
        {
            _localPlayerPreparation = Player.Local.GetComponent<Preparation>();
            _localPlayerPreparation.IsReady.ValueChanged += OnValueChange;

            ValidateTabs();
        }

        private void OnValueChange(bool previousValue, bool newValue) => ValidateTabs();
        private void ValidateTabs()
        {
            if (_localPlayerPreparation.IsReady.Value)
            {
                _readyTab.Enable();
            }
            else
            {
                _notReadyTab.Enable();
            }
        }
    }
}
