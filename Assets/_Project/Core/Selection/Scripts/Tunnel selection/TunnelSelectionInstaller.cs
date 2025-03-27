using Zenject;

namespace Core.Selection.Tunnels
{
    public class TunnelSelectionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            NoiseContainerSelection noiseContainerSelection = new();
            Container.Bind<NoiseContainerSelection>().FromInstance(noiseContainerSelection).AsSingle();
        }
    }
}
