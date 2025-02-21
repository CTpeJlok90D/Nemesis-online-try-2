using Zenject;

namespace Core.Selection.Rooms
{
    public class RoomSelectionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<RoomSelection>().FromNew().AsSingle();
        }
    }
}
