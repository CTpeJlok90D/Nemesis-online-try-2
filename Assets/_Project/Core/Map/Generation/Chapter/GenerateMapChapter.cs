using Core.Scenarios;

namespace Core.Maps.Generation.Chapter
{
    public class GenerateMapChapter : IChapter
    {
        private MapGenerator _mapGenerator;
        public event IChapter.EndedListener Ended;

        public GenerateMapChapter(MapGenerator mapGenerator)
        {
            _mapGenerator = mapGenerator;
        }

        public void Begin()
        {
            _mapGenerator.Generate();
            Ended?.Invoke(this);
        }
    }
}
