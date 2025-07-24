using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Core.Maps
{
    public class MapInstaller : MonoInstaller
    {
        [FormerlySerializedAs("_map")] [SerializeField] private Ship _ship;

        public override void InstallBindings()
        {
            Container
                .Bind<Ship>()
                .FromInstance(_ship)
                .AsSingle();
        }
    }
}
