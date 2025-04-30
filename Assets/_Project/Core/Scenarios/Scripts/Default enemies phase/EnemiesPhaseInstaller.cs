using Core.AlienAttackDecks;
using Core.PlayerTablets;
using Core.TimeTracks;
using UnityEngine;
using Zenject;

namespace Core.Scenarios.EnemiesPhase
{
    public class EnemiesPhaseScenarioInstaller : MonoBehaviour, IContainsScenario
    {
        [SerializeField] private TimeTrack _mainTimeTrack;
        [SerializeField] private TimeTrack _selfDestructionTimeTrack;
        [SerializeField] private ScenarioLauncher _playerPhaseLauncher;

        [Inject] private AlienAttackDeck _alienAttackDeck;
        [Inject] private PlayerTabletList _playerTabletList;
        public Scenario Scenario { get; private set; }

        private void Awake()
        {
            IChapter[] chapters =
            {
                new MoveTrackChapter(_mainTimeTrack),
                new MoveTrackChapter(_selfDestructionTimeTrack),
                new EnemiesAttackPhase(_alienAttackDeck, _playerTabletList),
                new LaunchScenarioChapter(_playerPhaseLauncher),
            };
            
            Scenario = new Scenario(chapters);
        }
    }
}
