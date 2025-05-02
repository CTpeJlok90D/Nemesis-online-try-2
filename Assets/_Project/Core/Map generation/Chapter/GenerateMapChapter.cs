using Core.Scenarios;
using Unity.Netcode;

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
            if (NetworkManager.Singleton.IsServer == false)
            {
                return;
            }

            _mapGenerator.Generate();
            Ended?.Invoke(this);
        }
    }
}
