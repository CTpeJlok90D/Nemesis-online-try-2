using Core.CharacterChoose;
using Core.CharacterInventorys;
using Core.LoadObservers;
using Core.Maps;
using Core.Maps.Generation;
using Core.Maps.Generation.Chapter;
using Core.Missions.Dealing;
using Core.OrderNumberDestributors;
using Core.PlayerTablets;
using Core.Scenarios.Default;
using UnityEngine;
using Zenject;

namespace Core.Scenarios
{
    public class GamePreparationScenarioInstaller : MonoBehaviour, IContainsScenario
    {
        [SerializeField] private OrderNumberDestributor _orderNumberDistributor;

        [SerializeField] private CharactersDealer _charactersDealer;

        [SerializeField] private MapGenerator _map;

        [SerializeField] private MissionsDealer _missionDealer;

        [SerializeField] private PawnPlacerConfig _pawnPlacerConfig;

        [SerializeField] private RoomCell _startRoom;

        [SerializeField] private ToggleGameObjectChapter _cameraChapter;

        [SerializeField] private ScenarioLauncher _playersPhaseScenarioLauncher;

        [Inject] private KitStartConfig _kitStartConfig;
        [Inject] private PlayerTabletList _playerTabletList;
        [Inject] private LoadObserver _loadObserver;
        
        public Scenario Scenario { get; private set; }
        
        public void Awake()
        {
            GenerateMapChapter generateMapChapter = new(_map);
            Delay delay = new(1.5f);

            IChapter[] chapters =
            {
                _cameraChapter,
                generateMapChapter,
                _orderNumberDistributor,
                _missionDealer,
                new AwaitOtherPlayers(_loadObserver),
                delay,
                new DealCharactersChapter(_charactersDealer),
                new PawnsPlacer(_playerTabletList, _pawnPlacerConfig, _startRoom, _kitStartConfig),
                new LaunchScenarioChapter(_playersPhaseScenarioLauncher),
                _cameraChapter,
            };

            Scenario = new(chapters);
        }
    }
}
