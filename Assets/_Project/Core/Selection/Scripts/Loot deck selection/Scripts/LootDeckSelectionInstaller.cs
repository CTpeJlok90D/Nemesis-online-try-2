using UnityEngine;
using Zenject;

namespace Core.Selection.LootDeckSelections
{
    public class LootDeckSelectionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            LootDeckSelection selection = new();
            Container.BindInstance(selection);
        }
    }
}
