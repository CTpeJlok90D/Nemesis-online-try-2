using Core.Characters.Infection;
using Core.Lobbies;
using Zenject;

namespace Core.AlienAttackDecks
{
    public class InfectionDeckConfigInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Lobby lobby = Container.Resolve<Lobby>();
            InfectionDeck.Config config = lobby.Configuration.InfectionDeckConfiguration;
            
            Container.Bind<InfectionDeck.Config>().FromInstance(config).AsSingle();
        }
    }
}
