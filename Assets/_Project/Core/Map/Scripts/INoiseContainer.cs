using Unity.Netcode;

namespace Core.Maps
{
    public interface INoiseContainer
    {
        public IReadOnlyReactiveField<bool> IsNoised { get; }

        public void Noise();

        public void Clear();
        
        public NetworkObject NetworkObject { get; }
    }
}
