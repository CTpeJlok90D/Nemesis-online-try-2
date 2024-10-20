using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Core.Players
{
    public class NicknameContainer : NetworkBehaviour
    {
#if UNITY_EDITOR
        private const string NICKNAME_FORMAT = "Player: {0}";
#endif

        [SerializeField] private string _nickname;
        private NetworkVariable<FixedString64Bytes> _networkNickname = new(writePerm:NetworkVariableWritePermission.Owner);

        public event Action<string> Changed;
        
        public string Nickname
        {
            get => _nickname;
            set
            {
#if UNITY_EDITOR
                gameObject.name = string.Format(NICKNAME_FORMAT, value);
#endif
                _networkNickname.Value = new FixedString64Bytes(value);
                _nickname = value;
            }
        }

        private void Start()
        {
            _nickname = _networkNickname.Value.ToString();
            Changed?.Invoke(_nickname);
#if UNITY_EDITOR
            gameObject.name = string.Format(NICKNAME_FORMAT, _nickname);
#endif
        }

        private void Awake()
        {
            _networkNickname.OnValueChanged = OnNicknameChange;
        }

        private void OnNicknameChange(FixedString64Bytes oldNickname, FixedString64Bytes newNickname) 
        {
            _nickname = newNickname.ToString();
            Changed?.Invoke(_nickname);
#if UNITY_EDITOR
            gameObject.name = string.Format(NICKNAME_FORMAT, _nickname);
#endif
        }
    }
}
