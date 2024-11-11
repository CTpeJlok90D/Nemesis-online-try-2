using AYellowpaper.SerializedCollections;
using Core.Characters;
using UnityEngine;
using Zenject;

namespace UI.CharacterSelection
{
    [CreateAssetMenu(menuName = "Game/UI/Character backgrounds dictionary")]
    [Icon("Assets/_Project/UI/Character selection/Editor/icons8-book-96.png")]
    internal class CharacterBackgroundDictionary : ScriptableObjectInstaller
    {
        [field: SerializeField] public SerializedDictionary<string, Sprite> Sprites { get; private set; }

        public override void InstallBindings()
        {
            Container
                .Bind<CharacterBackgroundDictionary>()
                .FromInstance(this)
                .AsSingle();
        }
    }
}
