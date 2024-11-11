using Core.CharacterChoose;
using Core.Characters;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.CharacterSelection
{
    public class ChooseButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private CharacterContainer _characterContainer;

        [Inject] private CharactersDealer _charactersDealer;

        public void OnPointerClick(PointerEventData eventData)
        {
            _charactersDealer.Selection.Choose(_characterContainer.Character);
        }
    }
}
