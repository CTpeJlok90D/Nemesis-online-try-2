using UnityEngine;
using Zenject;

namespace Core.Starter
{
    public class ActivatorInstaller : MonoInstaller
    {
        [SerializeField] private string _gameSceneName;

        public override void InstallBindings()
        {
            Activator activator = new(_gameSceneName);
            Container.Bind<Activator>().FromInstance(activator).AsSingle();
        }
    }
}
