using Core.PlayerTablets;
using UnityEngine;
using Zenject;

namespace Core.Scenarios.PlayersPhase
{
    public class PlayersPhaseScenarioInstaller : MonoInstaller
    {
        [SerializeField] private ActionPointsGiver _actionPointsGiver;

        public override void InstallBindings()
        {
            PlayerTabletListInstaller playerTabletListInstaller = ProjectContext.Instance.GetComponentInChildren<PlayerTabletListInstaller>();
            PlayerTabletList playerTabletList = playerTabletListInstaller.PlayerTabletList;
            Delay delay = new(1);

            IChapter[] chapters = new IChapter[]
            {
                new DrawCardsChapter(playerTabletList),
                new MoveFirstPlayer(_actionPointsGiver),
                new PlayersActionPhase(playerTabletList, _actionPointsGiver),
            };

            Scenario scenario = new(chapters);

            Container.Bind<Scenario>().FromInstance(scenario);
        }

    }
}
