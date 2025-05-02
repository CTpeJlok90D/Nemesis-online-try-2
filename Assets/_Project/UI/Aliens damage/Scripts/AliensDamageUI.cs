using System.Collections.Generic;
using Core.AlienAttackDecks;
using UnityEngine;

namespace UI.AliensDamages
{
    [DefaultExecutionOrder(5)]
    public class AliensDamageUI : MonoBehaviour
    {
        [SerializeField] private GameObject _damageIndicator_PREFAB;
        [SerializeField] private AlienCardsHealthHandler _healthHandler;
        [SerializeField] private Transform _indicatorsParent;

        private List<GameObject> _damageIndicatorInstances = new();
        
        private void OnEnable()
        {
            _healthHandler.Damage.Changed += OnDamageChange;
        }

        private void Start()
        {
            UpdateDamageView();
        }

        private void OnDisable()
        {
            _healthHandler.Damage.Changed -= OnDamageChange;
        }

        private void OnDamageChange(int oldValue, int newValue)
        {
            UpdateDamageView();
        }

        private void UpdateDamageView()
        {
            while (_damageIndicatorInstances.Count > _healthHandler.Damage.Value)
            {
                GameObject instance = _damageIndicatorInstances[^1];
                _damageIndicatorInstances.Remove(instance);
                Destroy(instance);
            }

            while (_damageIndicatorInstances.Count < _healthHandler.Damage.Value)
            {
                GameObject instance = Instantiate(_damageIndicator_PREFAB, _indicatorsParent);
                _damageIndicatorInstances.Add(instance);
            }
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_healthHandler == null)
            {
                _healthHandler = GetComponentInParent<AlienCardsHealthHandler>();
            }
        }
#endif
    }
}
