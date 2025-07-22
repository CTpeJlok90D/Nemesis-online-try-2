using UnityEngine;
using Zenject;

namespace Core.Lobbies
{
    public class LobbyInstaller : MonoInstaller
    {
        [SerializeField] private Lobby _lobby_PREFAB;

        public Lobby Lobby { get; private set; }

        public override void InstallBindings()
        {
            Lobby = Instantiate(_lobby_PREFAB);
            DontDestroyOnLoad(Lobby.NetworkObject);

            Container
                .Bind<Lobby>()
                .FromInstance(Lobby)
                .AsSingle();
        }
    }
}
