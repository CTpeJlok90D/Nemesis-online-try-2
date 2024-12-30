using Core.PlayerTablets;
using Zenject;

namespace Core.Scenarios.PlayersPhase
{
    public class PlayersPhaseScenarioInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            PlayerTabletListInstaller playerTabletListInstaller = ProjectContext.Instance.GetComponentInChildren<PlayerTabletListInstaller>();
            PlayerTabletList playerTabletList = playerTabletListInstaller.PlayerTabletList;

            IChapter[] chapters = new IChapter[]
            {
                new DrawCardsChapter(playerTabletList),
            };

            Scenario scenario = new(chapters);

            Container.Bind<Scenario>().FromInstance(scenario);
        }
    }
}
