using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.SelectionBase
{
    public class Selection<T> : IReadOnlyCollection<T>
    {
        public delegate void SelectionChangedListener();

        private List<T> _selection;

        private int _maxCount;

        public event SelectionChangedListener SelectionChanged;

        public Selection(int maxCount)
        {
            _maxCount = maxCount;
        }

        public int Count => _selection.Count;

        public IReadOnlyCollection<T> Elements => _selection;

        public virtual void Add(T value)
        {
            _selection.Add(value);
            if (_selection.Count > _maxCount)
            {
                T element = _selection.ElementAt(0);
                Remove(element);
            }

            SelectionChanged?.Invoke();
        }

        public virtual void Remove(T value)
        {
            _selection.Remove(value);
            SelectionChanged?.Invoke();
        }

        public void Clear()
        {
            foreach (T item in _selection.ToArray())
            {
                Remove(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _selection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _selection.GetEnumerator();
        }
    }
}
