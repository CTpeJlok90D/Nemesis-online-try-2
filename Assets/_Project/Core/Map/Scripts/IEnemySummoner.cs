using Cysharp.Threading.Tasks;

namespace Core.Maps
{
    public interface IEnemySummoner
    {
        public UniTask<RoomContent> SummonIn(RoomCell roomCell);
    }
}
