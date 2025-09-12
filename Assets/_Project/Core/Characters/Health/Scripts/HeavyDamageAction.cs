using System.Linq;
using UnityEngine;

namespace Core.Characters.Health
{
    [RequireComponent(typeof(HeavyDamage))]
    public abstract class HeavyDamageAction : MonoBehaviour
    {
        private HeavyDamage _self;
        public HeavyDamage Self => _self;

        public bool IsDouble => Self.Owner.HeavyDamages.Count(damage => damage.ID == Self.ID) > 1;

        private void Awake()
        {
            _self = GetComponent<HeavyDamage>();
        }

        private void OnEnable()
        {
            _self.IsTreated.Changed += OnTreatedChanged;
        }

        private void OnDisable()
        {
            _self.IsTreated.Changed -= OnTreatedChanged;
        }

        private void OnTreatedChanged(bool oldValue, bool newValue)
        {
            if (newValue && newValue != oldValue)
            {
                OnTreaded();
            }
        }

        protected abstract void OnTreaded();
    }
}