using UnityEngine;
using Zenject;

namespace Core.AlienAttackDecks
{
    public class AlienAttackDeckInstaller : MonoInstaller
    {
        [SerializeField] private AlienAttackDeck _alienAttackDeck;
        public override void InstallBindings()
        {
            Container.Bind<AlienAttackDeck>().FromInstance(_alienAttackDeck).AsSingle();
        }
    }
}
