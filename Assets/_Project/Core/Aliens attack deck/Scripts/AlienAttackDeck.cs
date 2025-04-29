using System.Collections.Generic;
using System.Linq;
using Core.TabletopRandom;
using Unity.Netcode;
using Zenject;

namespace Core.AlienAttackDecks
{
    public class AlienAttackDeck : NetworkBehaviour
    {
        [Inject] private AlienAttackDeckConfig _enemiesConfig;

        private Bag<AlienAttackCard> _cards;

        private void Awake()
        {
            Refresh();
        }

        public AlienAttackCard PickOne()
        {
            if (_cards.Items.Count == 0)
            {
                Refresh();
            }
            
            AlienAttackCard result = _cards.PickOne();
            return result;
        }

        public AlienAttackCard[] Pick(int amount = 1)
        {
            List<AlienAttackCard> result = new();

            while (result.Count < amount)
            {
                result.Add(PickOne());
            }
            
            return result.ToArray();
        }

        private void Refresh()
        {
            AlienAttackCard[] cards = _enemiesConfig.Cards.ToArray();
            _cards = new(cards);
        }
    }
}
