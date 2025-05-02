using System;
using Core.CharacterInventories;
using UnityEngine;
using Zenject;

namespace Core.Lobbies
{
    public class KitStartConfigInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            try
            {
                Lobby lobby = Container.Resolve<Lobby>();
                KitStartConfig kitStartConfig = lobby.Configuration.KitStartConfig;
                
                Container.Bind<KitStartConfig>().FromInstance(kitStartConfig).AsSingle();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
