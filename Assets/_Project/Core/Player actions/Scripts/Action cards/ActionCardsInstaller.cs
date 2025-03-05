using UnityEngine;
using Zenject;

namespace Core.ActionsCards
{
    public class ActionCardsInstaller : MonoInstaller
    {
        [SerializeField] private ActionCardsDecksDictionary ActionCardsDeck;

        public override void InstallBindings()
        {
            Container.Bind<ActionCardsDecksDictionary>().FromInstance(ActionCardsDeck).AsSingle();
        }
    }
}
