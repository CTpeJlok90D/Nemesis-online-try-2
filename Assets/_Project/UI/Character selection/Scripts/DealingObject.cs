using Core.CharacterChoose;
using Core.Players;
using UnityEngine;
using Zenject;

namespace UI.CharacterSelection
{
    public class DealingObject : MonoBehaviour
    {
        [SerializeField] private GameObject _object; 

        [Inject] private CharactersDealer _characterDealer;

        private void OnEnable()
        {
            _characterDealer.IsDealing.Changed += OnDealChange;
        }

        private void OnDisable()
        {
            _characterDealer.IsDealing.Changed -= OnDealChange;
        }

        private void OnDealChange(bool previousValue, bool newValue)
        {
            _object.SetActive(newValue);
        }
    }
}
