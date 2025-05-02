using Core.Characters.Infection;
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
            Game.InfectionDeck = _infectionDeck;
        }
    }
    public static class Game
    {
        public static InfectionDeck InfectionDeck { get; internal set; }
    }
}

