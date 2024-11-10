using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Characters;
using ModestTree;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.CharacterChoose
{
    public class CharactersSelection : NetworkBehaviour, IEnumerable<Character>
    {
        public delegate void ChangedListener();

        private NetworkList<FixedString64Bytes> _networkCharacters;

        private Character _choosedCharacter;

        private List<Character> _characters;

        public event ChangedListener Changed;

        private void Awake()
        {
            _networkCharacters = new();
            _characters = new();
        }

        private void OnEnable()
        {
            _networkCharacters.OnListChanged += OnListChange;
        }

        private void OnDisable()
        {
            _networkCharacters.OnListChanged -= OnListChange;
        }

        public Character[] Get()
        {
            return _characters.ToArray();
        }

        internal async Task<Character> Choose(Character[] characters)
        {
            try
            {
                if (NetworkManager.IsServer == false)
                {
                    throw new NotServerException("Only server can set characters to choose!");
                }

                if (characters.IsEmpty())
                {
                    throw new ArgumentException("Array is empty!");
                }
                
                _networkCharacters.Clear();
                FixedString64Bytes[] charactersLoadkeys = characters.Select(x => new FixedString64Bytes((string)x.AddessablePath.RuntimeKey)).ToArray();

                foreach (FixedString64Bytes key in charactersLoadkeys)
                {
                    _networkCharacters.Add(key);
                    Debug.Log(key);
                }

                while (_choosedCharacter == null)
                {
                    await Awaitable.NextFrameAsync();
                }

                Character choose = _choosedCharacter;
                return choose;
            }
            catch (ArgumentException e)
            {
                Debug.LogException(e);
                return null;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return characters[0];
            }
        }

        internal void Clear()
        {
            _networkCharacters.Clear();
            _characters.Clear();
        }

        public void Choose(Character character)
        {
            if (IsOwner == false)
            {
                throw new Exception("Only owner can choose characters");
            }

            Choose_RPC(character);
        }

        [Rpc(SendTo.Server)]
        private void Choose_RPC(Character character)
        {
            _choosedCharacter = character;
        }

        private void OnListChange(NetworkListEvent<FixedString64Bytes> changeEvent)
        {
            List<Character> charactersBuffer = new();
            int loadCount = _networkCharacters.Count;

            foreach (FixedString64Bytes result in _networkCharacters)
            {
                string key = result.ToString();
                AsyncOperationHandle<Character> handle = Addressables.LoadAssetAsync<Character>(key);

                handle.Completed += handle => 
                {
                    charactersBuffer.Add(handle.Result);

                    if (charactersBuffer.Count == loadCount)
                    {
                        Changed?.Invoke();
                        _characters = charactersBuffer.ToList();
                    }
                };
            }
        }

        public IEnumerator<Character> GetEnumerator()
        {
            return _characters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (Character character in _characters)
            {
                yield return character;
            }
        }
    }
}
