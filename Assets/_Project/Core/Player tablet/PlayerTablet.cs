using System;
using System.Linq;
using System.Threading.Tasks;
using Core.ActionsCards;
using Core.CharacterInventorys;
using Core.Characters;
using Core.Characters.Health;
using Core.Missions;
using Core.Players;
using Core.Maps.CharacterPawns;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Core.PlayerTablets
{
    public class PlayerTablet : NetworkBehaviour, IContainsPlayer
    {
        public Inventory SmallItemsInventory { get; private set; }
        public Inventory BigItemsInventory { get; private set; }
        public ActionCardsDeck ActionCardsDeck { get; private set; }
        public NicknameContainer NicknameContainer { get; private set; }
        public CharacterHealth Health { get; private set; }

        [Inject] private PlayerTabletList _playerTabletList;

        [Inject] private ActionCardsDecksDictionary _actionCardsDecksDictionary;

        private bool _haveResult;

        private ToBookResult _result;

        private NetVariable<NetworkObjectReference> _linkedCharacterPawn;

        public NetBehaviourReference<Player> PlayerReference { get; private set; }
        public NetVariable<Character> Character { get; private set; }
        public NetVariable<int> ActionCount { get; private set; }
        public NetVariable<bool> IsPassed { get; private set; }
        public NetVariable<int> OrderNumber { get; private set; }
        public NetScriptableObjectList4096<Mission> Missions { get; private set; }
        
        public Player Player => PlayerReference.Reference;
        public bool IsEmpty => PlayerReference.Reference == null;
        public string Nickname => NicknameContainer.Value;
        
        public delegate void LinkPawnHandler(PlayerTablet sender);
        public event LinkPawnHandler PawnLinked;

        public CharacterPawn CharacterPawn
        {
            get
            {
                CharacterPawn result = null;
                
                if (_linkedCharacterPawn.Value.TryGet(out NetworkObject netObject))
                {
                    result = netObject.GetComponent<CharacterPawn>();
                }

                return result;
            }
        }

        private void Awake()
        {
            _linkedCharacterPawn = new();
            PlayerReference = new();
            Character = new();
            ActionCount = new();
            IsPassed = new();
            OrderNumber = new();
            Missions = new();
        }

        public bool CanBookIt(Player player) => IsEmpty && _playerTabletList.ActiveTablets.Any(x => x.PlayerReference.Reference == player) == false;

        public void LinkPawn(CharacterPawn characterPawn)
        {
            _linkedCharacterPawn.Value = characterPawn.NetworkObject;
            
            SmallItemsInventory = characterPawn.SmallItemsInventory;
            BigItemsInventory = characterPawn.BigItemsInventory;
            ActionCardsDeck = characterPawn.ActionCardsDeck;
            
            ActionCardsDeck.InitializeDeck(_actionCardsDecksDictionary[characterPawn.LinkedCharacter.Id]);

            if (Health != null)
            {
                Health.Dead -= OnPawnDeath;
            }
            Health = characterPawn.Health;
            Health.Dead += OnPawnDeath;
            
            PawnLinked?.Invoke(this);
        }

        private void OnPawnDeath(CharacterHealth characterHealth)
        {
            _playerTabletList.Remove(this);
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
            if (NetworkManager.Singleton == null || NetworkManager.Singleton.IsServer == false)
            {
                return;
            }

            if (PlayerReference.Reference == player)
            {
                PlayerReference.Reference = null;
            }
        }

        public async Task<ToBookResult> ToBookItFor(Player player)
        {
            try
            {
                if (CanBookIt(player) == false)
                {
                    throw new Exception("Can't book this tablet");
                }

                ToBookItFor_RPC(player.NetworkObject);
                while (_haveResult == false)
                {
                    await Awaitable.NextFrameAsync();
                }
                _haveResult = false;

                return _result;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return ToBookResult.Failure;
            }
        }

        public void Pass()
        {
            Pass_RPC();
        }

        [Rpc(SendTo.Server)]
        private void Pass_RPC()
        {
            IsPassed.Value = true;
        }

        [Rpc(SendTo.Server)]
        private void ToBookItFor_RPC(NetworkObjectReference playerReference)
        {
            if (playerReference.TryGet(out NetworkObject playerObject) == false)
            {
                return;
            }

            Player player = playerObject.GetComponent<Player>();
            if (IsEmpty)
            {
                PlayerReference.Reference = player;
                NicknameContainer = player.GetComponent<NicknameContainer>();
                SendResult_RPC(ToBookResult.Success, RpcTarget.Single(player.OwnerClientId, RpcTargetUse.Persistent));

                return;
            }
            SendResult_RPC(ToBookResult.Failure, RpcTarget.Single(player.OwnerClientId, RpcTargetUse.Persistent));
        }

        [Rpc(SendTo.SpecifiedInParams)]
        private void SendResult_RPC(ToBookResult result, RpcParams rpcParams = default)
        {
            _result = result;
            _haveResult = true;
        }

        public void LeavePlayerTablet()
        {
            LeavePlayerTablet_RPC();
        }

        [Rpc(SendTo.Server)]
        private void LeavePlayerTablet_RPC()
        {
            PlayerReference.Reference = null;
        }

    #if UNITY_EDITOR
        [CustomEditor(typeof(PlayerTablet))]
        private class CEditor : Editor
        {
            private PlayerTablet tablet => target as PlayerTablet;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (tablet.NetworkManager == null)
                {
                    return;
                }

                if (tablet.NetworkManager.IsServer)
                {
                    tablet.Character.Value = EditorGUILayout.ObjectField(tablet.Character.Value, typeof(Character), false) as Character;
                }
                else
                {
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField(tablet.Character.Value, typeof(Character), false);
                    GUI.enabled = true;
                }

                GUI.enabled = false;
                EditorGUILayout.ObjectField(tablet.PlayerReference.Reference, typeof(Player), false);
                EditorGUILayout.IntField("Order number: ", tablet.OrderNumber.Value);
                foreach (Mission mission in tablet.Missions)
                {
                    EditorGUILayout.ObjectField(mission, typeof(Mission), false);
                }
                GUI.enabled = true;
            }
        }
    #endif
    }

    public enum ToBookResult { Success, Failure }

}
