using System;
using UnityEngine;
using Zenject;

namespace Core.ActionsCards
{
    public class ActionCardsInstaller : MonoInstaller
    {
        [SerializeField] private ActionCardsDecksDictionary ActionCardsDeck;

        public override void InstallBindings()
        {
            try
            {
                Container.Bind<ActionCardsDecksDictionary>().FromInstance(ActionCardsDeck).AsSingle();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
