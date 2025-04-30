namespace Core.Scenarios
{
    public class LaunchScenarioChapter : IChapter
    {
        private ScenarioLauncher _scenarioLauncher;
        public LaunchScenarioChapter(ScenarioLauncher scenarioLancher)
        {
            _scenarioLauncher = scenarioLancher;
        }

        public event IChapter.EndedListener Ended;

        public void Begin()
        {
            _scenarioLauncher.Launch();
            Ended?.Invoke(this);
        }
    }
}
