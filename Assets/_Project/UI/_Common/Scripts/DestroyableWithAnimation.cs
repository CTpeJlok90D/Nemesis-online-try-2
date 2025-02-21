using UnityEngine;

namespace UI.Common
{
    public class DestroyableWithAnimation : MonoBehaviour, IDestroyable
    {
        [SerializeField] private Animator _animator;

        [SerializeField] private string _triggerName = "Destroyed";

        [SerializeField] private float _destroyDelay = 0.5f;

        public void Destroy()
        {
            _animator.SetTrigger(_triggerName);
            Destroy(gameObject, _destroyDelay);
        }
    }
}
