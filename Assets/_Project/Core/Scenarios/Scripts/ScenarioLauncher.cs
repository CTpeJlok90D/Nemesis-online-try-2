using System.Linq;
using TNRD;
using UnityEngine;

namespace Core.Scenarios
{
    public class ScenarioLauncher : MonoBehaviour
    {
        public delegate void ScenarioCompletedListener();

        [SerializeField] private bool _launchOnStart;

        [SerializeField] private SerializableInterface<IContainsScenario> _scenario;

        private int _currentChapterIndex = 0;

        private bool _isLaunched;

        public IChapter ActiveChapter
        {
            get { return _scenario.Value.Scenario.Chapters[_currentChapterIndex]; }
        }

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
                throw new ScenarioException("Scenario is already launched");
            }

            if (_scenario.Value.Scenario.Chapters.Any() == false)
            {
                throw new ScenarioException("Scenario is empty");
            }

            ActiveChapter.Ended += OnActiveChapterEnd;
            ActiveChapter.Begin();
        }

        private void OnActiveChapterEnd(IChapter sender)
        {
            ActiveChapter.Ended -= OnActiveChapterEnd;

            _currentChapterIndex++;

            if (_currentChapterIndex < _scenario.Value.Scenario.Chapters.Length)
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
