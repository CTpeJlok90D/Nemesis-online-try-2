using System;
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

        private static int LaunchCount = 0;

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

        private void Update()
        {
            LaunchCount = 0;
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

            if (LaunchCount >= 10)
            {
                throw new Exception("Too many. TOOOOOOOOOOOO");
            }

            LaunchCount++;
            
            _currentChapterIndex = 0;

            ActiveChapter.Ended += OnActiveChapterEnd;
            try
            {
                ActiveChapter.Begin();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Cant execute chapter {ActiveChapter}. Skipping");
                Debug.LogException(ex);
                OnActiveChapterEnd(ActiveChapter);
            }
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
                _isLaunched = false;
                _currentChapterIndex = 0;
                ScenarioCompleted?.Invoke();
            }
        }
    }
}
