using Core.CharacterChoose;
using Core.LoadObservers;
using Core.Maps.Generation;
using Core.Maps.Generation.Chapter;
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

        private Scenario _scenario;
        
        public override void InstallBindings()
        {
            LoadObserverInstaller loadObserverInstaller = ProjectContext.Instance.GetComponentInChildren<LoadObserverInstaller>();
            GenerateMapChapter generateMapChapter = new(_map);
            LoadObserver loadObserver = loadObserverInstaller.CharacterDealer;    

            IChapter[] chapters =
            {
                new AwaitOtherPlayers(loadObserver),
                generateMapChapter,
                _orderNumberDestributor,
                new DealCharactersChapter(_charactersDealer),
            };

            _scenario = new(chapters);

            Container
                .Bind<Scenario>()
                .FromInstance(_scenario);
        }
    }
}
