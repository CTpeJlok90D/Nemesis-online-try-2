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
            if (_selection == null)
            {
                return;
            }
            
            _target.gameObject.SetActive(_selection.IsActive);
        }
    }
}
