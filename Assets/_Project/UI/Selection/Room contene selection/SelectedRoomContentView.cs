using System.Linq;
using Core.Maps;
using Core.Selection.RoomContentSelections;
using UnityEngine;
using Zenject;

namespace UI.Selection.RoomConeneSelection
{
    public class SelectedRoomContentView : MonoBehaviour
    {
        [SerializeField] private RoomContent _roomCell;
        [SerializeField] private GameObject _selected;
        [SerializeField] private GameObject _canBeSelected;

        [Inject] private RoomContentSelection _roomSelection;

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