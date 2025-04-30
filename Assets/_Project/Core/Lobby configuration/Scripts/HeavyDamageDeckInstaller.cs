using Core.Characters.Health;
using Zenject;

namespace Core.Lobbies
{
    public class HeavyDamageDeckInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Lobby lobby = Container.Resolve<Lobby>();
            HeavyDamageDeck.Config config = lobby.Configuration.HeavyDamageDeckConfig;
            HeavyDamageDeck deck = new(config);
            
            Container.BindInstance(deck).AsSingle();
        }
    }
}
