using System.Linq;
using Core.PlayerTablets;
using Core.Starter;
using UnityEngine;
using Zenject;

namespace Core.ResultEvaluators
{
    public class ResultEvaluator : MonoBehaviour
    {
        [Inject] private PlayerTabletList _playerTabletList;
        [Inject] private Activator _activator;

        private void OnEnable()
        {
            _playerTabletList.TabletRemoved += OnTabletRemove;
        }

        private void OnDisable()
        {
            _playerTabletList.TabletRemoved -= OnTabletRemove;
        }

        private void OnTabletRemove(PlayerTabletList sender, PlayerTablet tablet)
        {
            if (_playerTabletList.Any() == false)
            {
                _ = _activator.StopGame();
            }
        }
    }
}
