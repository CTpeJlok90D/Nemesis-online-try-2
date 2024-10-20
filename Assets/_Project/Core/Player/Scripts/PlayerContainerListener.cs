using UnityEngine;

namespace Core.Players
{
    public abstract class PlayerContainerListener : MonoBehaviour
    {
        [SerializeField] private PlayerContainer _container;

        public PlayerContainer PlayerContainer => _container;

        protected virtual void OnEnable()
        {
            _container.Changed += OnPlayerChange;
            if (_container.HavePlayer) 
            {
                OnPlayerChange(_container);
            }
        }

        protected virtual void OnDisable()
        {
            _container.Changed -= OnPlayerChange;
        }

        protected virtual void OnPlayerChange(Player player) { }
    }
}
