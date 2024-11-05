using UnityEngine;
using Zenject;

namespace Core.Lobbies
{
    public class LobbyInstaller : MonoInstaller
    {
        [SerializeField] private Lobby _lobby_PREFAB;

        public override void InstallBindings()
        {
            Lobby instance = Instantiate(_lobby_PREFAB);
            DontDestroyOnLoad(instance);

            Container
                .Bind<Lobby>()
                .FromInstance(instance)
                .AsSingle();
        }
    }
}
