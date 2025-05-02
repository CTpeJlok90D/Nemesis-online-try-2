using System.Linq;
using Core.PlayerTablets;
using Unity.Netcode;
using UnityEngine;

namespace Core.Scenarios.PlayersPhase
{
    public class PlayersActionPhase : IChapter
    {
        private PlayerTabletList _playerTabletList;
        private ActionPointsGiver _actionPointsGiver;

        public event IChapter.EndedListener Ended;

        public PlayersActionPhase(PlayerTabletList playerTabletsList, ActionPointsGiver actionPointsGiver)
        {
            _playerTabletList = playerTabletsList;
            _actionPointsGiver = actionPointsGiver;
        }

        public void Begin()
        {
            if (NetworkManager.Singleton.IsServer == false)
            {
                return;
            }

            foreach (PlayerTablet playerTablet in _playerTabletList)
            {
                playerTablet.IsPassed.Value = false;
                playerTablet.IsPassed.Changed += OnIsPassedChange;
            }

            Debug.Log("Action phase started");
            _actionPointsGiver.Give();
        }

        private void OnIsPassedChange(bool previousValue, bool newValue)
        {
            if (_playerTabletList.All(x => x.IsPassed.Value))
            {
                Debug.Log("Players actions phase is ended");
                Ended?.Invoke(this);
            }
        }
    }
}
