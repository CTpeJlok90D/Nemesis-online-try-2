using UnityEngine;
using Zenject;

namespace Core.Starter
{
    public class ActivatorInstaller : MonoInstaller
    {
        [SerializeField] private string _gameSceneName;

        public Activator Activator { get; private set; }

        public override void InstallBindings()
        {
            Activator = new(_gameSceneName);
            Container.Bind<Activator>().FromInstance(Activator).AsSingle();
        }
    }
}
