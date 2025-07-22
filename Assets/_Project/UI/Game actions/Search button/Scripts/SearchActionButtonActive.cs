using Core.Characters.Actions;
using Core.Maps;
using Core.PlayerTablets;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace UI.SearchActionButton
{
    public class SearchActionButtonActive : MonoBehaviour
    {
        [SerializeField] private GameObject _target;
        
        private RoomCell RoomCell => PlayerTablet.Local.CharacterPawn.RoomContent.Owner;
        private PlayerTablet LocalTablet => PlayerTablet.Local;
        
        private void Update()
        {
            if (NetworkManager.Singleton == null || NetworkManager.Singleton.IsClient == false)
            {
                return;
            }
            
            if (LocalTablet == null || LocalTablet.CharacterPawn == null)
            {
                return;
            }
            
            _target.SetActive(SimpleSearch.RoomIsValidToLoot(RoomCell) && SimpleSearch.ExecutorHaveCard(LocalTablet));
        }
    }
}
