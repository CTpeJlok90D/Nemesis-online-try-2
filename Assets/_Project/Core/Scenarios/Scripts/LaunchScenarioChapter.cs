using UnityEngine;

namespace Core.Scenarios
{
    public class LaunchScenarioChapter : IChapter
    {
        private readonly ScenarioLauncher _scenarioLauncher;
        public LaunchScenarioChapter(ScenarioLauncher scenarioLauncher)
        {
            _scenarioLauncher = scenarioLauncher;
        }

        public event IChapter.EndedListener Ended;

        public void Begin()
        {
            Debug.Log($"Launching scenario: {_scenarioLauncher}");
            _scenarioLauncher.Launch();
            Ended?.Invoke(this);
        }
    }
}
