using Core.CharacterChoose;
using Core.LoadObservers;
using Core.Maps.Generation;
using Core.Maps.Generation.Chapter;
using Core.Missions.Dealing;
using Core.OrderNumberDestributors;
using UnityEngine;
using Zenject;

namespace Core.Scenarios
{
    public class DefaultScenarioInstaller : MonoInstaller
    {
        [SerializeField] private OrderNumberDestributor _orderNumberDestributor;

        [SerializeField] private CharactersDealer _charactersDealer;

        [SerializeField] private MapGenerator _map;

        [SerializeField] private MissionsDealer _missionDealer;

        private Scenario _scenario;
        
        public override void InstallBindings()
        {
            LoadObserverInstaller loadObserverInstaller = ProjectContext.Instance.GetComponentInChildren<LoadObserverInstaller>();

            GenerateMapChapter generateMapChapter = new(_map);
            LoadObserver loadObserver = loadObserverInstaller.CharacterDealer;    
            Delay delay = new(2f);

            IChapter[] chapters =
            {
                generateMapChapter,
                _orderNumberDestributor,
                _missionDealer,
                new AwaitOtherPlayers(loadObserver),
                delay,
                new DealCharactersChapter(_charactersDealer),
            };

            _scenario = new(chapters);

            Container
                .Bind<Scenario>()
                .FromInstance(_scenario);
        }
    }
}
