using Zenject;
using ISelection = Core.SelectionBase.ISelection;

namespace Core.Selection.Tunnels
{
    public class TunnelSelectionISelectionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            NoiseContainerSelection noiseContainerSelection = Container.Resolve<NoiseContainerSelection>();
            Container.Bind<ISelection>().FromInstance(noiseContainerSelection).AsSingle();
        }
    }
}
