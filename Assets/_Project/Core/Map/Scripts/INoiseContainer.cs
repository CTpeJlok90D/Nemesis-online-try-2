using System;
using Unity.Netcode;

namespace Core.Maps
{
    public interface INoiseContainer
    {
        public IReadOnlyReactiveField<bool> IsNoised { get; }

        internal void Noise();

        public void Clear();
        
        public NetworkObject NetworkObject { get; }
    }
}
