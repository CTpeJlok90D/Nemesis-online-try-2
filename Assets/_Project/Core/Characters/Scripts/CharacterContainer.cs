using UnityEngine;

namespace Core.Characters
{
    public class CharacterContainer : MonoBehaviour, IContainsCharacter
    {
        [field: SerializeField] public Character Character { get; private set; }

        public CharacterContainer Instantiate(Character character, Transform parent = null)
        {
            gameObject.SetActive(false);
            CharacterContainer characterContainer = Instantiate(this, parent);
            gameObject.SetActive(true);

            characterContainer.Character = character;
            characterContainer.gameObject.SetActive(true);

            return characterContainer;
        }
    }
}
