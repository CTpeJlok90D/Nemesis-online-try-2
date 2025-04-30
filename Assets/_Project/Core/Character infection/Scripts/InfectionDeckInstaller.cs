using UnityEngine;
using Zenject;

namespace Core.Characters.Infection
{
    public class InfectionDeckInstaller : MonoInstaller
    {
        [SerializeField] private InfectionDeck _infectionDeck;
        public override void InstallBindings()
        {
            Container.Bind<InfectionDeck>().AsSingle();
        }
    }
}
