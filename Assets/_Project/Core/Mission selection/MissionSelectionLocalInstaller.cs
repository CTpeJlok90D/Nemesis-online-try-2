using Core.SelectionBase;
using Zenject;

namespace Core
{
    public class MissionSelectionLocalInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            MissionSelection missionSelection = Container.Resolve<MissionSelection>();

            Container
                .Bind<ISelection>()
                .FromInstance(missionSelection)
                .AsSingle();
        }
    }
}