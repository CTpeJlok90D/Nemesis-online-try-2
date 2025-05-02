using System.Collections.Generic;
using System.Linq;
using Core.ActionsCards;
using Core.Aliens;
using Core.Maps.CharacterPawns;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Core.AlienAttackDecks
{
    public class SurpriseAttack : MonoBehaviour
    {
        [SerializeField] private Enemy _enemy;

        [Inject] private PlayerTabletList _playerTabletList;
        [Inject] private NetworkManager _networkManager;
        
        private void Start()
        {
            if (_networkManager.IsServer == false)
            {
                return;
            }
            
            _ = Attack();
        }

        private async UniTask Attack()
        {
            CharacterPawn[] pawnsInRoom = _enemy.RoomContent.Owner.GetContentWith<CharacterPawn>().ToArray();

            if (pawnsInRoom.Length == 0)
            {
                return;
            }
            
            CharacterPawn target = pawnsInRoom.First();
            IReadOnlyCollection<ActionCard> targetHand = await target.ActionCardsDeck.GetHand();
            foreach (CharacterPawn pawn in pawnsInRoom)
            {
                IReadOnlyCollection<ActionCard> hand = await pawn.ActionCardsDeck.GetHand();

                if (hand.Count < targetHand.Count)
                {
                    target = pawn;
                    targetHand = hand;
                }
            }

            if (targetHand.Count < _enemy.LinkedToken.Value.AttackReaction)
            {
                Debug.Log($"{_enemy} suddenly attacks {target}. Attacks reaction:{_enemy.LinkedToken.Value.AttackReaction}, Cards on hand: {targetHand.Count}");
                PlayerTablet playerTablet = _playerTabletList.First(x => x.CharacterPawn == target);
                _enemy.Attack(playerTablet);
            }
            else
            {
                Debug.Log($"{_enemy} does not make a surprise attack on {target}. Attacks reaction:{_enemy.LinkedToken.Value.AttackReaction}, Cards on hand: {targetHand.Count}");
            }
        }
    }
}
