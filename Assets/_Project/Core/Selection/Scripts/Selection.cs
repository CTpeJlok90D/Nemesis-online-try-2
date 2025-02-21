using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Core.SelectionBase
{
    public class Selection<T> : IReadOnlyCollection<T>
    {
        public delegate void SelectionChangedListener();

        private List<T> _selection = new();

        private ReactiveField<int> _maxCount = new(0);

        public event SelectionChangedListener SelectionChanged;

        public int Count => _selection.Count;

        public IReadOnlyCollection<T> Elements => _selection;

        public event IReadOnlyReactiveField<int>.ChangedListener MaxCountChanged
        {
            add => _maxCount.Changed += value;
            remove => _maxCount.Changed -= value;
        }

        public int MaxCount 
        {
            get
            {
                return _maxCount.Value;
            }
            set
            {
                _maxCount.Value = value;
                Clear();
            }
        }

        public virtual void Add(T value)
        {
            _selection.Add(value);
            if (_selection.Count > MaxCount)
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
            _selection.Clear();
            SelectionChanged?.Invoke();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T item in _selection)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (T item in _selection)
            {
                yield return item;
            }
        }
    }
}
