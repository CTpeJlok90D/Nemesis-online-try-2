using Core.LootDecks;
using Zenject;

namespace Core.Lobbies
{
    public class LootDeckConfigInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Lobby lobby = Container.Resolve<Lobby>();
            LootDecksConfig config = lobby.Configuration.LootDecksConfig;

            Container.Bind<LootDecksConfig>().FromInstance(config);
        }
    }
}
