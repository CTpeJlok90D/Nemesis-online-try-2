using Core.Characters;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.CharacterSelection
{
    public class CharacterBackground : MonoBehaviour
    {
        [SerializeField] private Image _image;

        [SerializeField] private CharacterContainer _characterContainer;

        [Inject] private CharacterBackgroundDictionary _charactersDictionary;

        private void Start()
        {
            UpdateBackground();
        }

        private void UpdateBackground()
        {
            if (_charactersDictionary.Sprites.ContainsKey(_characterContainer.Character.Id))
            {
                _image.sprite = _charactersDictionary.Sprites[_characterContainer.Character.Id];
            }
        }
    }
}