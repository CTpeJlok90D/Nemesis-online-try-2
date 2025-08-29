using Core.Characters;
using TMPro;
using TNRD;
using UnityEngine;

namespace UI
{
    public class CharacterName : MonoBehaviour
    {
        [SerializeField] private TMP_Text _caption;
        [SerializeField] private SerializableInterface<IContainsCharacter> _characterContainer;

        private Character Character => _characterContainer.Value.Character;
        
        private void OnEnable()
        {
            UpdateName();
        }

        private void UpdateName()
        {
            _caption.text = Character.Id;
        }
    }
}