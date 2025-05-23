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

namespace Core.CharacterChoose
{
    public class CharactersDealer : NetworkBehaviour
    {
        public delegate void СhoiceIsProvidedListener();

        [SerializeField] private CharactersSelection _charactersSelection;

        private PlayerTabletList _playerTabletList;

        public CharactersSelection Selection => _charactersSelection;

        private Lobby _lobby;

        public NetVariable<bool> IsDealing { get; private set; } = new (false, writePerm: NetworkVariableWritePermission.Server);

        private List<Character> _characters;

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
                if (IsDealing.Value)
                {
                    throw new Exception("Dealer is already dealing!");
                }

                if (NetworkManager.Singleton.IsServer == false)
                {
                    throw new NotServerException("Only server can start characters deal!");
                }

                IsDealing.Value = true;

                PlayerTablet[] orderedTablets = _playerTabletList.OrderBy(x => x.OrderNumber.Value).ToArray();

                _characters = _lobby.Configuration.Characters.ToList();
                foreach (PlayerTablet tablet in orderedTablets)
                {
                    Character[] charactersToChooseFrom = GetRandomCharacters(ChooseCharactersCount, _characters.ToArray());

                    Selection.NetworkObject.ChangeOwnership(tablet.PlayerReference.Reference.OwnerClientId);
                    Character choose = await Selection.Choose(charactersToChooseFrom);
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
                result.RemoveAt(UnityEngine.Random.Range(0, result.Count));
            }

            return result.ToArray();
        }
        
#if UNITY_EDITOR
        [CustomEditor(typeof(CharactersDealer))]
        private class CEditor : Editor
        {
            public CharactersDealer CharactersDealer => target as CharactersDealer;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (CharactersDealer.NetworkManager == null)
                {
                    return;
                }

                if (CharactersDealer.NetworkManager.IsServer == false || CharactersDealer.IsDealing.Value)
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
                    GUI.enabled = false;
                }

                GUILayout.Label($"Characters count: {CharactersDealer.Selection.Count()}");
                GUILayout.Label($"Is dealing: {CharactersDealer.IsDealing.Value}");

                foreach (Character character in CharactersDealer.Selection)
                {
                    if (GUILayout.Button(character.Id))
                    {
                        CharactersDealer.Selection.Choose(character);
                    }
                }

                GUI.enabled = true;
            }
        }
#endif
    }
}
