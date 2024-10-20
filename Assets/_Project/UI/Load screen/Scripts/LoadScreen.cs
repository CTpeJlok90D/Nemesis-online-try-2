using System.Threading.Tasks;
using UnityEngine;

namespace UI.Loading
{
    public class LoadScreen : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string _loadingStartedTriggerName = "loading started";
        [SerializeField] private string _loadingEndedTriggerName = "loading ended";

        public async Task StartLoading(Task loadTask) 
        {
            _animator.SetTrigger(_loadingStartedTriggerName);
            await loadTask;
            _animator.SetTrigger(_loadingEndedTriggerName);
        }
    }
}
