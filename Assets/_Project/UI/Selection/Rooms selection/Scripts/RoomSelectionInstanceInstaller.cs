using Core.Selection.Rooms;
using Core.SelectionBase;
using Zenject;

namespace UI.Selection.Rooms
{
    public class RoomSelectionInstanceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            RoomSelection roomSelection = Container.Resolve<RoomSelection>();
            Container.Bind<ISelection>().FromInstance(roomSelection).AsSingle();
        }
    }
}
