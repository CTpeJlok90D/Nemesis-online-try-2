using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Unity.Netcode.Custom
{
    public class NetScriptableObjectList4096<T> : NetVariable<FixedString4096Bytes>, IEnumerable<T>, IList<T> 
                                         where T : ScriptableObject, INetworkSerializable, IEquatable<T>, INetScriptableObjectArrayElement<T>
    {
        public delegate void ListChangedListener(NetScriptableObjectList4096<T> sender);

        private const string SEPARATOR = "_|_";
       
        private List<T> _cashedElements = new();

        public bool IsSyncing { get; private set; }

        private int _cashIndex = 0;

        private Dictionary<string, AsyncOperationHandle<T>> _loadedValues = new();

        public override FixedString4096Bytes Value 
        {
            get => throw new Exception("Cant change value. Use elements property to change array"); 
            set => throw new Exception("Cant change value. Use elements property to change array"); 
        }

        public int Count => Keys.Length;

        public string[] Keys 
        {
            get
            {
                string[] keys;
                if (string.IsNullOrEmpty(base.Value.ToString()))
                {
                    keys = new string[0];
                }
                else
                {
                    keys = base.Value.ToString().Split(SEPARATOR).ToArray();
                }
                return keys;
            }
        }

        public bool IsReadOnly => false;

        public T this[int i] 
        {
            get 
            {
                return _cashedElements[i]; 
            } 
            set  
            {
                List<string> keys = Keys.ToList();
                keys[i] = value.Net.RuntimeLoadKey;
                base.Value = string.Join(SEPARATOR, keys);
            }
        }

        public event ListChangedListener ListChanged;

        public async Task Sync()
        {
            while (IsSyncing)
            {
                await Awaitable.NextFrameAsync();
            }
        }

        public async Task<T[]> GetElements()
        {
            try
            {
                await Sync();
                return _cashedElements.ToArray();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public void SetElements(T[] values)
        {
            List<string> keys = new();
            foreach (T key in values)
            {
                keys.Add(key.Net.RuntimeLoadKey);
            }
            base.Value = string.Join(SEPARATOR, keys);
        }

        protected override void OnValueChange(FixedString4096Bytes previousValue, FixedString4096Bytes newValue)
        {
            base.OnValueChange(previousValue, newValue);
            CashValues();
        }

        private void CashValues()
        {
            _cashIndex++;
            int linkedCashIndex = _cashIndex;

            List<T> result = new();

            int keysToLoadCount = Keys.Length;
            IsSyncing = true;

            if (keysToLoadCount == 0)
            {
                _cashedElements = new List<T>();
                IsSyncing = false;
            }

            foreach (string loadKey in Keys)
            {
                string loadKeyCopy = loadKey;
                AsyncOperationHandle<T> handle;
                if (_loadedValues.ContainsKey(loadKey))
                {
                    handle = _loadedValues[loadKey];
                }
                else
                {
                    AssetReferenceT<T> tokenReference = new(loadKey);
                    handle = tokenReference.LoadAssetAsync();
                    _loadedValues.Add(loadKey, handle);
                }

                if (handle.IsDone)
                {
                    result.Add(handle.Result);
                    RemoveKey(ref keysToLoadCount, result);
                    continue;
                }

                handle.Completed += (loadedHandle) => 
                {
                    if (linkedCashIndex != _cashIndex)
                    {
                        return;
                    }

                    result.Add(loadedHandle.Result);
                    RemoveKey(ref keysToLoadCount, result);
                };
            }
        }

        private void RemoveKey(ref int keysToLoadCount, List<T> result)
        {
            keysToLoadCount--;
            if (keysToLoadCount <= 0)
            {
                _cashedElements = result.ToList();
                IsSyncing = false;
                ListChanged?.Invoke(this);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (IsSyncing)
            {
                throw new InvalidOperationException("Can not get enumerator while list is syncing");
            }

            return _cashedElements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (IsSyncing)
            {
                throw new InvalidOperationException("Can not get enumerator while list is syncing");
            }

            return _cashedElements.GetEnumerator();
        }

        public void Add(T value)
        {
            List<string> keys = Keys.ToList();
            keys.Add(value.Net.RuntimeLoadKey);
            base.Value = string.Join(SEPARATOR, keys);
        }

        public void AddRange(IEnumerable<T> values)
        {
            List<string> keys = Keys.ToList(); 
            keys.AddRange(from x in values select x.Net.RuntimeLoadKey);
            base.Value = string.Join(SEPARATOR, keys);
        }

        public int IndexOf(T item)
        {
            return _cashedElements.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            List<string> keys = Keys.ToList();
            keys.Insert(index, item.Net.RuntimeLoadKey);
            base.Value = string.Join(SEPARATOR, keys);
        }

        public void RemoveAt(int index)
        {
            List<string> keys = Keys.ToList();
            keys.RemoveAt(index);
            base.Value = string.Join(SEPARATOR, keys);
        }

        public void Clear()
        {
            base.Value = new();
        }

        public bool Contains(T item)
        {
            return _cashedElements.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _cashedElements.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (_cashedElements.Contains(item) == false)
            {
                return false;
            }

            Regex regex = new Regex(Regex.Escape(item.Net.RuntimeLoadKey));
            base.Value = regex.Replace(base.Value.ToString(), "", 1);
            return true;
        }

        public void RemoveRange(IEnumerable<T> values)
        {
            List<string> keys = Keys.ToList();

            foreach (T value in values)
            {
                keys.Remove(value.Net.RuntimeLoadKey);
            }

            base.Value = string.Join(SEPARATOR, keys);
        }
    }
}
