using UnityEngine;
using Zenject;

namespace UI.Loading
{
    public class LoadScreenInstaller : MonoInstaller
    {
        [SerializeField] private LoadScreen _loadScreen_PREFAB;
        private LoadScreen _loadScreenInstance;

        public override void InstallBindings()
        {
            _loadScreenInstance = Instantiate(_loadScreen_PREFAB);
            DontDestroyOnLoad(_loadScreenInstance);
            Container.Bind<LoadScreen>().FromInstance(_loadScreenInstance).AsSingle();
        }
    }
}
