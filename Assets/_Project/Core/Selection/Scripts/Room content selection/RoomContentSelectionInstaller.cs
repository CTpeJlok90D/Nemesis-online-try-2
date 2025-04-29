using Zenject;

namespace Core.Selection.RoomContentSelections
{
    public class RoomContentSelectionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            RoomContentSelection roomContainer = new();
            Container.BindInstance(roomContainer).AsSingle();
        }
    }
}
