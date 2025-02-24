using System;
using System.Linq;
using Core.Maps;
using Core.Selection.Rooms;
using Core.SelectionBase;
using UnityEngine;
using Zenject;

namespace UI.Selection.Rooms
{
    public class SelectedRoomView : MonoBehaviour
    {
        [SerializeField] private RoomCell _roomCell;
        [SerializeField] private GameObject _target;

        [Inject] private RoomSelection _roomSelection;

        private void OnEnable()
        {
            _roomSelection.SelectionChanged += OnSelectionChanged;
            UpdateSelectionView();
        }

        private void OnDisable()
        {
            _roomSelection.SelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged(ISelection sender)
        {
            UpdateSelectionView();
        }

        private void UpdateSelectionView()
        {
            _target.SetActive(_roomSelection.Contains(_roomCell));
        }
    }
}