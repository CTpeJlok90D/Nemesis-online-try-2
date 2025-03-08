using System.Linq;
using Core.Maps;
using Core.Selection.Rooms;
using UnityEngine;
using Zenject;

namespace UI.Selection.Rooms
{
    public class SelectedRoomView : MonoBehaviour
    {
        [SerializeField] private RoomCell _roomCell;
        [SerializeField] private GameObject _selected;
        [SerializeField] private GameObject _canBeSelected;

        [Inject] private RoomsSelection _roomSelection;

        private void Update()
        {
            UpdateSelectionView();
        }
        
        private void UpdateSelectionView()
        {
            _selected.SetActive(_roomSelection.Contains(_roomCell));
            _canBeSelected.SetActive(_roomSelection.CanSelect(_roomCell));
        }
    }
}