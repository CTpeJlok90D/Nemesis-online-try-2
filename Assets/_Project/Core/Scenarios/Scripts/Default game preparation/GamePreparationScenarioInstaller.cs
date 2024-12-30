using Core.CharacterChoose;
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
    public class GamePreparationScenarioInstaller : MonoInstaller
    {
        [SerializeField] private OrderNumberDestributor _orderNumberDestributor;

        [SerializeField] private CharactersDealer _charactersDealer;

        [SerializeField] private MapGenerator _map;

        [SerializeField] private MissionsDealer _missionDealer;

        [SerializeField] private PawnPlacerConfig _pawnPlacerConfig;

        [SerializeField] private RoomCell _startRoom;

        [SerializeField] private ToggleGameObjectChapter _cameraChapter;

        [SerializeField] private ScenarioLauncher _scenarioLauncher;

        private Scenario _scenario;
        
        public override void InstallBindings()
        {
            LoadObserverInstaller loadObserverInstaller = ProjectContext.Instance.GetComponentInChildren<LoadObserverInstaller>();
            PlayerTabletListInstaller playerTabletListInstaller = ProjectContext.Instance.GetComponentInChildren<PlayerTabletListInstaller>();

            GenerateMapChapter generateMapChapter = new(_map);
            LoadObserver loadObserver = loadObserverInstaller.CharacterDealer;    
            Delay delay = new(3f);

            IChapter[] chapters =
            {
                _cameraChapter,
                generateMapChapter,
                _orderNumberDestributor,
                _missionDealer,
                new AwaitOtherPlayers(loadObserver),
                delay,
                new DealCharactersChapter(_charactersDealer),
                new PawnsPlacer(playerTabletListInstaller.PlayerTabletList, _pawnPlacerConfig, _startRoom),
                new LaunchScenarioChapter(_scenarioLauncher),
                _cameraChapter,
            };

            _scenario = new(chapters);

            Container
                .Bind<Scenario>()
                .FromInstance(_scenario);
        }
    }
}
