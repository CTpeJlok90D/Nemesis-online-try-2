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

        [Inject] private PlayerTabletList _playerTabletList;
        
        private RoomCell RoomCell => _playerTabletList.Local.CharacterPawn.RoomContent.Owner;
        
        private void Update()
        {
            if (NetworkManager.Singleton.IsClient == false)
            {
                return;
            }
            
            if (_playerTabletList.Local == null || _playerTabletList.Local.CharacterPawn == null)
            {
                return;
            }
            
            _target.SetActive(SimpleSearch.RoomIsValidToLoot(RoomCell) && SimpleSearch.ExecutorHaveCard(_playerTabletList.Local));
        }
    }
}
