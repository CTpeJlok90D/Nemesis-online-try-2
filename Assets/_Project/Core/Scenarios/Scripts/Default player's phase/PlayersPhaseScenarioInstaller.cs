using Core.PlayerTablets;
using UnityEngine;

namespace Core.Scenarios.PlayersPhase
{
    public class PlayersPhaseScenarioInstaller : MonoBehaviour, IContainsScenario
    {
        [SerializeField] private ScenarioLauncher _gameEndScenario;
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
            if (PlayerTablet.ActiveTablets.Count > 0)
            {
                _enemiesPhaseScenarioLauncher.Launch();
            }
            else
            {
                _gameEndScenario.Launch();
            }
        }
    }
}
