using UnityEngine;
using Zenject;

namespace Core.Characters
{
    public class CharacterContainer : MonoBehaviour, IContainsCharacter
    {
        [field: SerializeField] public Character Character { get; private set; }

        public CharacterContainer Instantiate(Character character, DiContainer diContainer = null, Transform parent = null)
        {
            gameObject.SetActive(false);
            CharacterContainer characterContainer = Instantiate(this, parent);
            gameObject.SetActive(true);

            characterContainer.Character = character;
            diContainer?.InjectGameObject(characterContainer.gameObject);
            characterContainer.gameObject.SetActive(true);

            return characterContainer;
        }
    }
}
