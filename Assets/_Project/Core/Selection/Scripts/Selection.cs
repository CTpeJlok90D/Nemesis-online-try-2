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
        private List<T> _selection = new();
        private List<T> _whiteList = new();
        private ReactiveField<int> _countToSelect = new(0);
        private bool _isSelectionInProgress;

        public event ISelection.SelectionChangedHandler SelectionStarted;
        public event ISelection.SelectionChangedHandler SelectionConfirmed;
        public event ISelection.SelectionChangedHandler SelectionCanceled;
        public event ISelection.SelectionChangedHandler SelectionChanged;
        public int Count => _selection.Count;
        public int SelectedCount => Count;
        public IReadOnlyCollection<T> Elements => _selection;
        public bool CanConfirmSelection => _selection.Count == RequiredCount && _isSelectionInProgress;
        public bool IsActive => _isSelectionInProgress;
        public IEnumerable<T> SelectionSource => _whiteList;
        public virtual bool OnlyUniqueItems => true; 

        public event IReadOnlyReactiveField<int>.ChangedListener MaxCountChanged
        {
            add => _countToSelect.Changed += value;
            remove => _countToSelect.Changed -= value;
        }


        public int RequiredCount 
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

        public bool CanSelect(T value)
        {
            if (IsActive == false)
            {
                return false;
            }

            if (_whiteList.Count > 0 && _whiteList.Contains(value) == false)
            {
                return false;
            }

            if (_selection.Contains(value))
            {
                return false;
            }
            
            return true;
        }

        public virtual void Add(T value)
        {
            if (IsActive == false)
            {
                throw new InvalidOperationException($"Cant add {value} to selection: Selection is not active");
            }
            
            if (_whiteList.Count > 0 &&_whiteList.Contains(value) == false)
            {
                throw new InvalidOperationException($"Cant add {value} to selection: {value} was not found in source");
            }
            
            if (_selection.Contains(value) && OnlyUniqueItems)
            {
                throw new InvalidOperationException($"Cant add {value} to selection: {value} already selected");
            }
            
            _selection.Add(value);
            if (_selection.Count > RequiredCount)
            {
                T element = _selection.ElementAt(0);
                Remove(element);
            }

            SelectionChanged?.Invoke(this);
        }

        internal async Task<T[]> Select(int count)
        {
            RequiredCount = count;
            _isSelectionInProgress = true;
            while (_isSelectionInProgress)
            {
                await Awaitable.NextFrameAsync();
            }

            T[] result =  _selection.ToArray();
            
            Clear();
            SelectionStarted?.Invoke(this);
            return result;
        }

        public void Cancel()
        {
            if (IsActive == false)
            {
                throw new InvalidOperationException("Can't cancel selection: selection is not active");
            }

            _isSelectionInProgress = false;
            _whiteList.Clear();
            Clear();
            SelectionCanceled?.Invoke(this);
        }

        public async Task<T[]> SelectFrom(IEnumerable<T> source, int count)
        {
            _whiteList = new(source);
            T[] result = await Select(count);
            _whiteList.Clear();
            
            return result;
        }

        public void Confirm()
        {
            if (_isSelectionInProgress == false)
            {
                throw new Exception("Can't confirm selection. Selection is not in progress");
            }

            if (_selection.Count == RequiredCount)
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
