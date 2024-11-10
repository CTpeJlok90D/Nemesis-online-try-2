using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Characters;
using Core.Lobbies;
using Core.PlayerTablets;
using Unity.Netcode;
using Unity.Netcode.Custom;
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

        public NetVariable<bool> IsDealing { get; private set; }

        private List<Character> _characters;

        public event СhoiceIsProvidedListener ChoiceIsProvided;

        public event OrderNumbersWereDistributedListener OrderNumbersWereDistributed;

        public int ChooseCharactersCount => _lobby.Configuration.ChooseCharactersCount;

        private void Awake()
        {
            IsDealing = new();
        }

        public void Init(Lobby lobby, PlayerTabletList playerTabletList)
        {
            _lobby = lobby;
            _playerTabletList = playerTabletList;
        }

        public override void OnNetworkSpawn()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                DistributeOrderNumbers();
            }
        }

        public async Task StartDeal()
        {       
            try
            {
                if (IsDealing.Value)
                {
                    throw new Exception("Dealer is already dealing!");
                }

                IsDealing.Value = true;
                if (NetworkManager.Singleton.IsServer == false)
                {
                    IsDealing.Value = false;
                    throw new NotServerException("Only server can start characters deal!");
                }

                PlayerTablet[] orderedTablets = _playerTabletList.OrderBy(x => x.OrderNumber.Value).ToArray();

                foreach (PlayerTablet tablet in orderedTablets)
                {
                    _characters = _lobby.Configuration.Characters.ToList();
                    Character[] characters = GetRandomCharacters(ChooseCharactersCount, _characters.ToArray());

                    Selection.NetworkObject.ChangeOwnership(tablet.PlayerReference.Reference.OwnerClientId);

                    Debug.Log(tablet.PlayerReference.Reference, tablet.PlayerReference.Reference);
                    Character choose = await Selection.Choose(characters);
                    Debug.Log(choose);

                    Selection.NetworkObject.ChangeOwnership(NetworkManager.ServerClientId);

                    if (choose == null)
                    {
                        return;
                    }

                    _characters.Remove(choose);
                    tablet.Character.Value = choose;
                }

                IsDealing.Value = false;
                Selection.Clear();
            }     
            catch (Exception e)
            {
                Debug.LogException(e);
                IsDealing.Value = false;
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

        private Character[] GetRandomCharacters(int count, Character[] selectFrom)
        {
            if (count <= 0)
            {
                throw new ArgumentException("Count must be grater then zero");
            }

            List<Character> result = selectFrom.ToList();

            if (result.Count < count)
            {
                return result.ToArray();
            }

            while (result.Count != count && result.Count != 0)
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
                if (CharactersDealer.NetworkManager == null || CharactersDealer.NetworkManager.IsServer == false || CharactersDealer.IsDealing.Value)
                {
                    GUI.enabled = false;
                }

                if (CharactersDealer.IsDealing.Value == false && GUILayout.Button("Deal"))
                {
                    _ = CharactersDealer.StartDeal();
                }

                GUI.enabled = true;

                if (CharactersDealer.Selection.IsOwner == false)
                {
                    return;
                }

                foreach (Character character in CharactersDealer.Selection)
                {
                    if (GUILayout.Button(character.name))
                    {
                        CharactersDealer.Selection.Choose(character);
                    }
                }

            }
        }
#endif
    }
}
