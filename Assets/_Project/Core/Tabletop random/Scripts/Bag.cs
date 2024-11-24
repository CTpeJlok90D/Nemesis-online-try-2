using System.Collections.Generic;
using UnityEngine;

namespace Core.TabletopRandom
{
    [System.Serializable]
    public class Bag<T>
    {
        [SerializeField] private List<T> _items;

        public IReadOnlyCollection<T> Items => _items;

        public Bag(IEnumerable<T> items)
        {
            _items = new(items);
        }

        public T PickOne()
        {
            if (_items.Count == 0)
            {
                throw new BagException("Bag is empty");
            }

            T result = _items[Random.Range(0, _items.Count)];
            _items.Remove(result);
            return result;
        }  

        public Bag<T> Clone()
        {
            Bag<T> result = new(Items);
            return result;
        }
    }
}
