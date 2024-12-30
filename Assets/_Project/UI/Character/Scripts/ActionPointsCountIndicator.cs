using System;
using System.Collections.Generic;
using AYellowpaper;
using Core.PlayerTablets;
using UnityEngine;

namespace UI.Characters
{
    public class ActionPointsCountIndicator : MonoBehaviour
    {
        [SerializeField] private InterfaceReference<IContainsPlayerTablet> _playerTablet;
        [SerializeField] private GameObject _actionPointsRoot;
        [SerializeField] private GameObject _actionPointIndicator_PREFAB;

        public PlayerTablet PlayerTablet => _playerTablet.Value.PlayerTablet;

        private List<GameObject> _actionPointsIndicatorsIntances = new();

        private void OnEnable()
        {
            PlayerTablet.ActionCount.Changed += OnActionCountChange;
        }

        private void OnDisable()
        {
            PlayerTablet.ActionCount.Changed -= OnActionCountChange;
        }

        private void OnActionCountChange(int previousValue, int newValue) => UpdatePoints();

        private void UpdatePoints()
        {
            _actionPointsRoot.gameObject.SetActive(PlayerTablet.ActionCount.Value >= 0);

            while (_actionPointsIndicatorsIntances.Count < PlayerTablet.ActionCount.Value)
            {
                _actionPointsIndicatorsIntances.Add(Instantiate(_actionPointIndicator_PREFAB, _actionPointsRoot.transform));
            }

            int i = 1;
            foreach (GameObject indicator in _actionPointsIndicatorsIntances)
            {
                indicator.SetActive(i <= PlayerTablet.ActionCount.Value);
                i++;
            }
        }
    }
}
