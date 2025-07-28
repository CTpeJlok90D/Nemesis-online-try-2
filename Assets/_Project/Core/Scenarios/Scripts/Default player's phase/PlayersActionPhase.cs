using System.Linq;
using Core.Maps.CharacterPawns;
using Core.PlayerTablets;
using Unity.Netcode;
using UnityEngine;

namespace Core.Scenarios.PlayersPhase
{
    public class PlayersActionPhase : IChapter
    {
        private readonly ActionPointsGiver _actionPointsGiver;

        public event IChapter.EndedListener Ended;

        public PlayersActionPhase(ActionPointsGiver actionPointsGiver)
        {
            _actionPointsGiver = actionPointsGiver;
        }

        public void Begin()
        {
            if (NetworkManager.Singleton.IsServer == false)
            {
                return;
            }

            CharacterPawn.Despawned += OnDespawn;

            foreach (PlayerTablet playerTablet in PlayerTablet.ActiveTablets)
            {
                playerTablet.IsPassed.Value = false;
                playerTablet.IsPassed.Changed += OnIsPassedChange;
            }

            Debug.Log("Action phase started");
            _actionPointsGiver.Give();
        }

        private void OnIsPassedChange(bool previousValue, bool newValue)
        {
            if (PlayerTablet.Instances.All(x => x.IsPassed.Value))
            {
                Debug.Log("Players actions phase is ended");
                Ended?.Invoke(this);
                CharacterPawn.Despawned -= OnDespawn;
            }
        }

        private void OnDespawn(CharacterPawn despawned)
        {
            if (PlayerTablet.ActiveTablets.Count == 0)
            {
                Ended?.Invoke(this);
            }
        }
    }
}
