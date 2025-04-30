using Core.PlayerTablets;
using UnityEngine;
using Zenject;

namespace Core.Scenarios.PlayersPhase
{
    public class PlayersPhaseScenarioInstaller : MonoBehaviour, IContainsScenario
    {
        [SerializeField] private ActionPointsGiver _actionPointsGiver;
        [SerializeField] private ScenarioLauncher _enemiesPhaseScenarioLauncher;

        [Inject] private PlayerTabletList _playerTabletList;
        
        public Scenario Scenario { get; private set; }
        
        public void Awake()
        {
            IChapter[] chapters = {
                new DrawCardsChapter(_playerTabletList),
                new MoveFirstPlayer(_actionPointsGiver),
                new PlayersActionPhase(_playerTabletList, _actionPointsGiver),
                new LaunchScenarioChapter(_enemiesPhaseScenarioLauncher)
            };

            Scenario = new(chapters);
        }
    }
}
