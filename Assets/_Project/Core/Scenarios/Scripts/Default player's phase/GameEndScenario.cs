using Core.Scenarios;
using UnityEngine;

namespace Core
{
    public class GameEndScenario : MonoBehaviour, IContainsScenario
    {
        [SerializeField] private ToggleGameObjectChapter _disableObjectsChapter;
        [SerializeField] private ToggleGameObjectChapter _enableObjectsChapter;
        public Scenario Scenario { get; private set; }

        private void Awake()
        {
            Delay delay15 = new(1.5f);

            IChapter[] chapters =
            {
                _disableObjectsChapter,
                delay15,
                _enableObjectsChapter
            };
            
            Scenario = new()
            {
                Chapters = chapters
            };
        }
    }
}