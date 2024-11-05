using UnityEngine;
using Zenject;
using CharacterReference = UnityEngine.AddressableAssets.AssetReferenceT<Core.Characters.Character>;

namespace Core.CharacterChoose
{
    public class CharactersDealerInstaller : MonoInstaller
    {
        [SerializeField] private CharacterReference[] _allCharacters;
        [SerializeField] private CharactersDealer _dealer;

        public override void InstallBindings()
        {
            Container.Bind<CharactersDealer>().FromInstance(_dealer).AsSingle();
        }
    }
}
