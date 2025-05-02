using System;
using Core.PlayerTablets;
using UnityEngine;
using Zenject;

namespace Core.Scenarios.PlayersPhase
{
    public class PlayersPhaseScenarioInstaller : MonoBehaviour, IContainsScenario
    {
        [SerializeField] private ActionPointsGiver _actionPointsGiver;
        [SerializeField] private ScenarioLauncher _enemiesPhaseScenarioLauncher;
        [SerializeField] private ScenarioLauncher _playersPhaseScenarioLauncher;

        [Inject] private PlayerTabletList _playerTabletList;
        
        public Scenario Scenario { get; private set; }
        
        public void Awake()
        {
            IChapter[] chapters = {
                new DrawCardsChapter(_playerTabletList),
                new MoveFirstPlayer(_actionPointsGiver),
                new PlayersActionPhase(_playerTabletList, _actionPointsGiver),
            };

            Scenario = new(chapters);
            
            _playersPhaseScenarioLauncher.ScenarioCompleted += OnScenarioComplete;
        }

        private void OnDestroy()
        {
            _playersPhaseScenarioLauncher.ScenarioCompleted -= OnScenarioComplete;
        }

        private void OnScenarioComplete()
        {
            _enemiesPhaseScenarioLauncher.Launch();
        }
    }
}
