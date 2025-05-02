using System;
using System.Linq;
using Core.AliensBags;
using Core.EventsDecks;
using Core.Maps;
using Core.Maps.Generation;
using Core.PlayerTablets;
using Core.TimeTracks;
using MyNamespace;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Core.Scenarios.EnemiesPhase
{
    [DefaultExecutionOrder(1)]
    public class EnemiesPhaseScenarioInstaller : MonoBehaviour, IContainsScenario
    {
        [SerializeField] private TimeTrack _mainTimeTrack;
        [SerializeField] private TimeTrack _selfDestructionTimeTrack;
        [SerializeField] private ScenarioLauncher _playerPhaseLauncher;
        [SerializeField] private ScenarioLauncher _enemiesPhaseScenarioLauncher;
        [SerializeField] private HiveDevelopment.Config _hiveDevelopment;
        [SerializeField] private RoomType _hiveRoomType;
        [SerializeField] private DefaultEnemySummoner _defaultEnemySummoner;
        [SerializeField] private MapGenerator _mapGenerator;
        [SerializeField] private EventsDeck _eventDeck;
        
        [Inject] private PlayerTabletList _playerTabletList;
        [Inject] private Map _map;
        [Inject] private DiContainer _diContainer;
        [Inject] private AliensBag _aliensBag;
        [Inject] private NetworkManager _networkManager;
        public Scenario Scenario { get; private set; }

        private void OnEnable()
        {
            _mapGenerator.Generated += OnGenerate;
        }

        private void OnDisable()
        {
            _mapGenerator.Generated -= OnGenerate;
        }

        private void OnGenerate(MapGenerator sender)
        {
            Init();
        }

        private void Init()
        {
            if (_networkManager.IsServer == false)
            {
                return;
            }
            
            RoomCell hiveRoom = _map.RoomCells.FirstOrDefault(x => x.Type == _hiveRoomType);

            if (hiveRoom == null)
            {
                throw new InvalidOperationException($"Hive room was not found");
            }
            
            HiveDevelopment hiveDevelopment = new(_hiveDevelopment, hiveRoom, _defaultEnemySummoner);
            _diContainer.Inject(hiveDevelopment);
            
            IChapter[] chapters =
            {
                new MoveTrackChapter(_mainTimeTrack),
                new MoveTrackChapter(_selfDestructionTimeTrack),
                new EnemiesAttackPhase(_playerTabletList),
                new FireDamage(),
                new PlayEventCard(_eventDeck, _aliensBag, _diContainer),
                hiveDevelopment
            };
            
            Scenario = new Scenario(chapters);

            _enemiesPhaseScenarioLauncher.ScenarioCompleted += OnScenarioComplete;
        }

        private void OnDestroy()
        {
            _enemiesPhaseScenarioLauncher.ScenarioCompleted -= OnScenarioComplete;
        }

        private void OnScenarioComplete()
        {
            _playerPhaseLauncher.Launch();
        }
    }
}
