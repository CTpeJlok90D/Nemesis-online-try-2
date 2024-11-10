using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace View.Common
{
    public class UnityEvents : MonoBehaviour
    {
        [SerializeField] private UnityEvent _awaked;
        [SerializeField] private UnityEvent _started;
        [SerializeField] private float _delay = 0.5f;
        [SerializeField] private UnityEvent _delayedStart;

        
        private void Awake()
        {
            _awaked.Invoke();
        }

        private IEnumerator Start()
        {
            _started.Invoke();
            yield return new WaitForSeconds(_delay);
            _delayedStart.Invoke();
        }
    }
}