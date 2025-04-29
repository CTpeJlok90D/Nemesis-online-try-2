using System;
using Core.Lobbies;
using UnityEngine;
using Zenject;

namespace Core.AlienAttackDecks
{
    public class AlienAttackDeckConfigInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            try
            {
                Lobby lobbyConfiguration = Container.Resolve<Lobby>();
                AlienAttackDeckConfig alienAttackDeckConfig = lobbyConfiguration.Configuration.AliensAttackDeckConfig;

                Container.Bind<AlienAttackDeckConfig>().FromInstance(alienAttackDeckConfig).AsSingle();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
