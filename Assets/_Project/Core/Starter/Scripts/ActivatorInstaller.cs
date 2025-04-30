using UnityEngine;
using Zenject;

namespace Core.Starter
{
    public class ActivatorInstaller : MonoInstaller
    {
        [SerializeField] private string _gameSceneName;
        [SerializeField] private string _lobbySceneName;

        public Activator Activator { get; private set; }

        public override void InstallBindings()
        {
            Activator = new(_gameSceneName, _lobbySceneName);
            Container.Bind<Activator>().FromInstance(Activator).AsSingle();
        }
    }
}
