using UnityEngine;
using UnityEngine.Events;

namespace Core.Players
{
    public class PlayerContainer : MonoBehaviour
    {
        public static implicit operator Player(PlayerContainer container) => container.Player;

        [SerializeField] UnityEvent<Player> _changed;

        private Player _player;
        public Player Player => _player;

        public bool HavePlayer => _player != null;
        public event UnityAction<Player> Changed 
        {
            add => _changed.AddListener(value);
            remove => _changed.RemoveListener(value);
        }

        public PlayerContainer Create(Player owner, Transform parent)
        {
            return Instantiate(this, parent).Init(owner);
        }

        public PlayerContainer Init(Player owner) 
        {
            _player = owner;
            _changed.Invoke(_player);
            gameObject.SetActive(true);
            return this;
        }

        private void OnEnable()
        {
            Player.Left += OnPlayerLeft;
        }

        private void OnDisable()
        {
            Player.Left -= OnPlayerLeft;
        }

        private void OnPlayerLeft(Player player)
        {
            if (_player == player) 
            {
                _player = null;
                _changed.Invoke(_player);
            }
        }
    }
}
