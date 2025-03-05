using Zenject;

namespace Core.Selection.Rooms
{
    public class RoomSelectionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            RoomsSelection selectionInstance = new();
            Container.Bind<RoomsSelection>().FromInstance(selectionInstance).AsSingle();
        }
    }
}
