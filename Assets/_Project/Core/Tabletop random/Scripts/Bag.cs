using System.Collections.Generic;
using UnityEngine;

namespace Core.TabletopRandom
{
    [System.Serializable]
    public class Bag<T>
    {
        [SerializeField] protected List<T> _items;

        public IReadOnlyCollection<T> Items => _items;

        public Bag(IEnumerable<T> items)
        {
            _items = new(items);
        }

        public virtual T PickOne()
        {
            if (_items.Count == 0)
            {
                throw new BagException("Bag is empty");
            }

            T result = _items[Random.Range(0, _items.Count)];
            _items.Remove(result);
            return result;
        }

        public virtual IEnumerable<T> Pick(int count)
        {
            while (count > 0)
            {
                yield return PickOne();
                count--;
            }
        }

        public Bag<T> Clone()
        {
            Bag<T> result = new(Items);
            return result;
        }
    }
}
