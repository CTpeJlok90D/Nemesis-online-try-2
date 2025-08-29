using Zenject;

namespace Core
{
    public class MissionSelectionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            MissionSelection missionSelection = new();

            Container
                .Bind<MissionSelection>()
                .FromInstance(missionSelection)
                .AsSingle();
        }
    }
}