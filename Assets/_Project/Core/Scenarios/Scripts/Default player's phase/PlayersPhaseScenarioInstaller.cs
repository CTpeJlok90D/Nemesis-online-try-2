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
        
        public Scenario Scenario { get; private set; }
        
        public void Awake()
        {
            IChapter[] chapters = {
                new DrawCardsChapter(),
                new MoveFirstPlayer(_actionPointsGiver),
                new PlayersActionPhase(_actionPointsGiver),
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
