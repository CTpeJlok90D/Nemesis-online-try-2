using System;
using System.Linq;
using Core.Maps;
using Core.Selection.Rooms;
using Core.SelectionBase;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace UI.Selection.Rooms
{
    public class SelectedRoomView : MonoBehaviour
    {
        [SerializeField] private RoomCell _roomCell;
        [SerializeField] private GameObject _selected;
        [SerializeField] private GameObject _canBeSelected;

        [Inject] private RoomsSelection _roomSelection;

        private void OnEnable()
        {
            _roomSelection.SelectionChanged += UpdateSelectionView;
            _roomSelection.SelectionStarted += UpdateSelectionView;
            _roomSelection.SelectionConfirmed += UpdateSelectionView;
            _roomSelection.SelectionCanceled += UpdateSelectionView;
            UpdateSelectionView();
        }

        private void OnDisable()
        {
            _roomSelection.SelectionStarted -= UpdateSelectionView;
            _roomSelection.SelectionConfirmed -= UpdateSelectionView;
            _roomSelection.SelectionChanged -= UpdateSelectionView;
            _roomSelection.SelectionCanceled -= UpdateSelectionView;
        }

        private void UpdateSelectionView(ISelection sender) => UpdateSelectionView();
        private void UpdateSelectionView()
        {
            _selected.SetActive(_roomSelection.Contains(_roomCell));
            _canBeSelected.SetActive(_roomSelection.CanSelect(_roomCell));
        }
    }
}