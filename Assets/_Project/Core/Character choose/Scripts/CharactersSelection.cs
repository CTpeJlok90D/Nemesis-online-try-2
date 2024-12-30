using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Characters;
using ModestTree;
using Unity.Netcode;
using UnityEngine;

namespace Core.CharacterChoose
{
    public class CharactersSelection : NetworkBehaviour, IEnumerable<Character>
    {
        public delegate void ChangedListener();

        public delegate void CharacterChoosedListener();

        private Character _choosedCharacter;

        private List<Character> _characters;

        public event ChangedListener GotCharacters;

        public event CharacterChoosedListener CharacterChoosed;

        private void Awake()
        {
            _characters = new();
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

                _characters = characters.ToList();
                SendCharactersToChooseFrom_RPC(characters);
                
                _choosedCharacter = null;
                while (_choosedCharacter == null && _characters.Contains(_choosedCharacter) == false)
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

        [Rpc(SendTo.Owner)]
        private void SendCharactersToChooseFrom_RPC(Character[] characters)
        {
            _characters = characters.ToList();
            GotCharacters?.Invoke();
        }

        [Rpc(SendTo.Server)]
        private void Choose_RPC(Character character)
        {
            _choosedCharacter = character;
            _characters.Clear();
            CharacterChoosedInvokeEvent_RPC();
        }

        [Rpc(SendTo.Everyone)]
        private void CharacterChoosedInvokeEvent_RPC()
        {
            CharacterChoosed?.Invoke();
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
