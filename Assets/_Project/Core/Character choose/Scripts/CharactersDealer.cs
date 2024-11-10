using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Characters;
using Core.Lobbies;
using Core.PlayerTablets;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Core.CharacterChoose
{
    public class CharactersDealer : NetworkBehaviour
    {

        public delegate void СhoiceIsProvidedListener();

        public delegate void OrderNumbersWereDistributedListener();

        [SerializeField] private CharactersSelection _charactersSelection;

        private PlayerTabletList _playerTabletList;

        public CharactersSelection Selection => _charactersSelection;

        private Lobby _lobby;

        public bool IsDealing { get; private set; }

        private List<Character> _characters;

        public event СhoiceIsProvidedListener ChoiceIsProvided;

        public event OrderNumbersWereDistributedListener OrderNumbersWereDistributed;

        public int ChooseCharactersCount => _lobby.Configuration.ChooseCharactersCount;

        public void Init(Lobby lobby, PlayerTabletList playerTabletList)
        {
            _lobby = lobby;
            _playerTabletList = playerTabletList;
        }

        public async Task StartDeal()
        {       
            try
            {
                if (IsDealing)
                {
                    throw new Exception("Dealer is already dealing!");
                }

                IsDealing = true;
                if (NetworkManager.Singleton.IsServer == false)
                {
                    IsDealing = false;
                    throw new NotServerException("Only server can start characters deal!");
                }

                DistributeOrderNumbers();

                PlayerTablet[] orderedTablets = _playerTabletList.OrderBy(x => x.OrderNumber.Value).ToArray();

                foreach (PlayerTablet tablet in orderedTablets)
                {
                    _characters = _lobby.Configuration.Characters.ToList();
                    Character[] characters = GetRandomCharacters(ChooseCharactersCount);

                    Selection.NetworkObject.ChangeOwnership(tablet.PlayerReference.Reference.OwnerClientId);
                    Character choose = await Selection.Choose(characters);
                    Selection.NetworkObject.ChangeOwnership(NetworkManager.ServerClientId);

                    _characters.Remove(choose);
                    tablet.Character.Value = choose;
                }
                IsDealing = false;
            }     
            catch (Exception e)
            {
                Debug.LogException(e);
                IsDealing = false;
            }
        }

        private void DistributeOrderNumbers()
        {
            List<int> numbers = new();
            for (int i = 1; i <= _playerTabletList.ActiveTablets.Length; i++)
            {
                numbers.Add(i);
            }

            foreach (PlayerTablet playerTablet in _playerTabletList)
            {
                int number = UnityEngine.Random.Range(0, _playerTabletList.Count());
                playerTablet.OrderNumber.Value = number;
                numbers.Remove(number);
            }

            OrderNumbersWereDistributed?.Invoke();
        }

        private Character[] GetRandomCharacters(int count)
        {
            List<Character> result = _characters.ToList();

            if (result.Count < count)
            {
                return result.ToArray();
            }

            while (result.Count != count || result.Count != 0)
            {
                result.Remove(result[UnityEngine.Random.Range(0, result.Count)]);
            }

            return result.ToArray();
        }

        private void RemoveCharacter(Character character)
        {
            _characters.Remove(character);
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(CharactersDealer))]
        private class CEditor : Editor
        {
            public CharactersDealer CharactersDealer => target as CharactersDealer;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (CharactersDealer.NetworkManager == null || CharactersDealer.NetworkManager.IsServer == false || CharactersDealer.IsDealing)
                {
                    GUI.enabled = false;
                }

                if (GUILayout.Button("Deal"))
                {
                    _ = CharactersDealer.StartDeal();
                }

                GUI.enabled = true;
            }
        }
#endif
    }
}
