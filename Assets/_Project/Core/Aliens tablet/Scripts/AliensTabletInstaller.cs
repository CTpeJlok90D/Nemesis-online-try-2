using UnityEngine;
using Zenject;

namespace Core.AliensTablets
{
    public class AliensTabletInstaller : MonoInstaller
    {
        [SerializeField] private AliensTablet _aliensTablet;

        public override void InstallBindings()
        {
            Container
                .Bind<AliensTablet>()
                .FromInstance(_aliensTablet);
        }
    }
}
