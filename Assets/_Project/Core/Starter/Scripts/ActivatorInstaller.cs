using UnityEngine;
using Zenject;

namespace Core.Starter
{
    public class ActivatorInstaller : MonoInstaller
    {
        public Activator Activator { get; private set; }

        public override void InstallBindings()
        {
            Container.Bind<Activator>().FromInstance(Activator.Singleton).AsSingle();
        }
    }
}
