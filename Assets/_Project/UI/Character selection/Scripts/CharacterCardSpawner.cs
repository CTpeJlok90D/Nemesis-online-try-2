using System;
using System.Collections.Generic;
using Core.CharacterChoose;
using Core.Characters;
using UnityEngine;
using Zenject;

namespace UI.CharacterSelection
{
    public class CharacterCardSpawner : MonoBehaviour
    {
        [SerializeField] private CharacterContainer _characterCard_PREFAB;
        
        [SerializeField] private Transform _cardsParent;

        [Inject] private CharactersDealer _characterDealer;
        
        [Inject] private DiContainer _diContainer;

        private List<CharacterContainer> _cardInstances = new();

        private void OnEnable()
        {
            _characterDealer.Selection.GotCharacters += OnGotCharacters;
            _characterDealer.IsDealing.Changed += OnIsDealingChanged;
            _characterDealer.Selection.CharacterChoosed += OnCharacterChoose;
        }

        private void OnDisable()
        {
            _characterDealer.Selection.GotCharacters -= OnGotCharacters;
            _characterDealer.IsDealing.Changed -= OnIsDealingChanged;
            _characterDealer.Selection.CharacterChoosed -= OnCharacterChoose;
        }

        private void OnCharacterChoose()
        {
            DestroyCards();
        }

        private void OnIsDealingChanged(bool previousValue, bool newValue)
        {
            if (newValue == false)
            {
                DestroyCards();
            }
        }

        private void OnGotCharacters()
        {
            DestroyCards();
            CreateCards();
        }

        private void DestroyCards()
        {
            foreach (CharacterContainer characterContainer in _cardInstances)
            {
                Destroy(characterContainer.gameObject);
            }

            _cardInstances.Clear();
        }

        private void CreateCards()
        {
            foreach (Character character in _characterDealer.Selection)
            {
                CharacterContainer characterContainer = _characterCard_PREFAB.Instantiate(character, _diContainer, _cardsParent);
                _cardInstances.Add(characterContainer);
            }
        }
    }
}
