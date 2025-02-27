using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.SelectionBase
{
    public class Selection<T> : IReadOnlyCollection<T>, ISelection
    {
        public delegate void SelectionChangedListener(Selection<T> sender);

        private List<T> _selection = new();
        private ReactiveField<int> _countToSelect = new(0);
        private bool _isSelectionInProgress;

        public event SelectionChangedListener SelectionStarted;
        public event SelectionChangedListener SelectionConfirmed;
        public event ISelection.SelectionChangedHandler SelectionChanged;
        public int Count => _selection.Count;
        public int SelectedCount => Count;
        public IReadOnlyCollection<T> Elements => _selection;
        public bool CanConfirmSelection => _selection.Count == CountToSelect && _isSelectionInProgress;

        public event IReadOnlyReactiveField<int>.ChangedListener MaxCountChanged
        {
            add => _countToSelect.Changed += value;
            remove => _countToSelect.Changed -= value;
        }


        public int CountToSelect 
        {
            get
            {
                return _countToSelect.Value;
            }
            private set
            {
                _countToSelect.Value = value;
                Clear();
            }
        }

        public virtual void Add(T value)
        {
            _selection.Add(value);
            if (_selection.Count > CountToSelect)
            {
                T element = _selection.ElementAt(0);
                Remove(element);
            }

            SelectionChanged?.Invoke(this);
        }

        public async Task<T[]> Select(int count)
        {
            CountToSelect = count;
            _isSelectionInProgress = true;
            while (_isSelectionInProgress)
            {
                await Awaitable.NextFrameAsync();
            }

            T[] result =  _selection.ToArray();
            
            Clear();
            return result;
        }

        public void Comfirm()
        {
            if (_isSelectionInProgress == false)
            {
                throw new Exception("Can't confirm selection. Selection is not in progress");
            }

            if (_selection.Count == CountToSelect)
            {
                _isSelectionInProgress = false;
            }
            else
            {
                throw new Exception("Can't confirm selection. Selection count is not equal to required count");
            }
        }

        public virtual void Remove(T value)
        {
            _selection.Remove(value);
            SelectionChanged?.Invoke(this);
        }

        public void Clear()
        {
            _selection.Clear();
            SelectionChanged?.Invoke(this);
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
