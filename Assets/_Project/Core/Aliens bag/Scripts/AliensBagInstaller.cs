using UnityEngine;
using Zenject;

namespace Core.AliensBags
{
    public class AliensBagInstaller : MonoInstaller
    {
        [SerializeField] private AliensBag _aliensBag;

        public override void InstallBindings()
        {
            Container
                .Bind<AliensBag>()
                .FromInstance(_aliensBag);
        }
    }
}
