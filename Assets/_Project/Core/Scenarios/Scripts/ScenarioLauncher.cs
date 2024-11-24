using ModestTree;
using UnityEngine;
using Zenject;

namespace Core.Scenarios
{
    public class ScenarioLauncher : MonoBehaviour
    {
        public delegate void ScenarioCompletedListener();

        [SerializeField] private bool _launchOnStart;

        [Inject] private Scenario _scenario;

        private int _currenctChapterIndex = 0;

        private bool _isLaunched;

        public IChapter ActiveChapter => _scenario.Chapters[_currenctChapterIndex];

        public event ScenarioCompletedListener ScenarioCompleted;

        private void Start()
        {
            if (_launchOnStart)
            {
                Launch();
            }
        }

        public void Launch()
        {
            if (_isLaunched)
            {
                new ScenarioException("Scenario is already launched");
            }

            if (_scenario.Chapters.IsEmpty())
            {
                new ScenarioException("Scenario is empty");
            }

            ActiveChapter.Ended += OnActiveChapterEnd;
            ActiveChapter.Begin();
        }

        private void OnActiveChapterEnd(IChapter sender)
        {
            ActiveChapter.Ended -= OnActiveChapterEnd;

            _currenctChapterIndex++;

            if (_currenctChapterIndex < _scenario.Chapters.Length)
            {
                ActiveChapter.Ended += OnActiveChapterEnd;
                ActiveChapter.Begin();
            }
            else
            {
                ScenarioCompleted?.Invoke();
            }
        }
    }
}
