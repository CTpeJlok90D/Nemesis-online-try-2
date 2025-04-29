using Core.SelectionBase;
using Zenject;

namespace Core.Selection.RoomContentSelections
{
    public class ISelectionRoomContentInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            RoomContentSelection roomContainer = Container.Resolve<RoomContentSelection>();
            Container.Bind<ISelection>().FromInstance(roomContainer).AsSingle();
        }
    }
}
