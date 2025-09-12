using System;
using Core.PlayerTablets;
using UnityEngine;

namespace UI
{
    public class LoaclPlayerIngureIndicator : MonoBehaviour
    {
        [SerializeField] private GameObject _indicator;
        [SerializeField] private int _injureIndex = 1;

        private void OnEnable()
        {
            UpdateActive();
            PlayerTablet.Local.Health.LightDamageCount.Changed += OnLightDamageCountChanged;
            PlayerTablet.LocalChanged += OnLocalPlayerChange;
        }

        private void OnDisable()
        {
            if (PlayerTablet.Local != null)
            {
                PlayerTablet.Local.Health.LightDamageCount.Changed -= OnLightDamageCountChanged;
                PlayerTablet.LocalChanged -= OnLocalPlayerChange;
            }
        }

        private void OnLocalPlayerChange(PlayerTablet old, PlayerTablet newValue) => UpdateActive();
        private void OnLightDamageCountChanged(int oldValue, int newValue) => UpdateActive();
        private void UpdateActive()
        {
            _indicator.SetActive(PlayerTablet.Local.Health.LightDamageCount.Value >= _injureIndex);
        }
    }
}