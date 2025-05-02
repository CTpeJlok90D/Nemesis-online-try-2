using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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

        private const string Separator = "_|_";
       
        private List<T> _cashedElements = new();
        
        public IReadOnlyList<T> CashedElements => _cashedElements;

        public bool IsSyncing { get; private set; }

        private int _cashIndex = 0;

        private Dictionary<string, AsyncOperationHandle<T>> _loadedValues = new();

        public NetScriptableObjectList4096(NetworkVariableReadPermission readPermission = NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission writePermission = NetworkVariableWritePermission.Server) : base(string.Empty, readPermission, writePermission)
        {
            Changed += OnListChange;
        }

        ~NetScriptableObjectList4096()
        {
            Changed -= OnListChange;
        }

        public override FixedString4096Bytes Value
        {
            get { throw new Exception("Cant change value. Use elements property to change array"); }
            set { throw new Exception("Cant change value. Use elements property to change array"); }
        }


        public int Count
        {
            get { return Keys.Length; }
        }

        public string[] Keys 
        {
            get
            {
                string[] keys;
                if (string.IsNullOrEmpty(base.Value.ToString()))
                {
                    keys = Array.Empty<string>();
                }
                else
                {
                    keys = base.Value.ToString().Split(Separator).ToArray();
                }
                return keys;
            }
            private set
            {
                base.Value = string.Join(Separator, value);
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
                Keys = keys.ToArray();
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

        public void SetElements(IEnumerable<T> values)
        {
            List<string> keys = new();
            foreach (T key in values)
            {
                keys.Add(key.Net.RuntimeLoadKey);
            }
            Keys = keys.ToArray();
        }

        private void OnListChange(FixedString4096Bytes previousValue, FixedString4096Bytes newValue)
        {
            _ = SyncValues();
        }

        private async UniTask SyncValues()
        {
            await CashValues();
            ListChanged?.Invoke(this);
        }

        private async UniTask CashValues()
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

                await handle.ToUniTask();

                if (linkedCashIndex != _cashIndex)
                {
                    return;
                }

                result.Add(handle.Result);
                RemoveKey(ref keysToLoadCount, result);
            }
        }

        private void RemoveKey(ref int keysToLoadCount, List<T> result)
        {
            keysToLoadCount--;
            if (keysToLoadCount <= 0)
            {
                _cashedElements = result.ToList();
                IsSyncing = false;
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
            Keys = keys.ToArray();
        }

        public void AddRange(IEnumerable<T> values)
        {
            List<string> keys = Keys.ToList(); 
            keys.AddRange(from x in values select x.Net.RuntimeLoadKey);
            Keys = keys.ToArray();
        }

        public int IndexOf(T item)
        {
            return _cashedElements.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            List<string> keys = Keys.ToList();
            keys.Insert(index, item.Net.RuntimeLoadKey);
            Keys = keys.ToArray();
        }

        public void RemoveAt(int index)
        {
            List<string> keys = Keys.ToList();
            keys.RemoveAt(index);
            Keys = keys.ToArray();
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

            List<string> keys = Keys.ToList();
            keys.Remove(item.Net.RuntimeLoadKey);
            Keys = keys.ToArray();
            return true;
        }

        public void RemoveRange(IEnumerable<T> values)
        {
            List<string> keys = Keys.ToList();

            foreach (T value in values)
            {
                keys.Remove(value.Net.RuntimeLoadKey);
            }

            Keys = keys.ToArray();
        }
    }
}
