using System;
using Core.SelectionBase;
using UnityEngine;
using Zenject;

namespace UI.Selection
{
    public class SelectionIsActiveObject : MonoBehaviour
    {
        [SerializeField] private GameObject _target;
        [Inject] private ISelection _selection;

        private void Update()
        {
            _target.gameObject.SetActive(_selection.IsActive);
        }
    }
}
