using System;
using System.Linq;
using Core.TabletopRandom;
using Unity.Netcode;

namespace Core.Characters.Health
{
    public class HeavyDamageDeck : Bag<HeavyDamage>
    {
        private Config _config;

        public HeavyDamageDeck(Config config) : base(config.DamageCards)
        {
            _config = config;
        }

        public override HeavyDamage PickOne()
        {
            if (_items.Count == 0)
            {
                _items = new(_config.DamageCards);
                
            }
            
            return base.PickOne();
        }

        [Serializable]
        public struct Config : INetworkSerializable
        {
            public HeavyDamage[] DamageCards;
            
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                int count = 0;
                if (serializer.IsWriter)
                {
                    count = DamageCards.Length;
                }
                
                serializer.SerializeValue(ref count);
                
                
                NetworkObjectReference[] objects = new NetworkObjectReference[count];
                if (serializer.IsWriter)
                {
                    objects = DamageCards.Select(x => new NetworkObjectReference(x.NetworkObject)).ToArray();
                }
                serializer.SerializeValue(ref objects);

                if (serializer.IsReader)
                {
                    DamageCards = objects.Select(x =>
                    {
                        x.TryGet(out NetworkObject result);
                        return result.GetComponent<HeavyDamage>();
                    }).ToArray();
                }
            }
        }
    }
}
