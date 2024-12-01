using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Aliens;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.AliensBags
{
    public class AliensBag : NetworkBehaviour
    {
        private const string SEPARATOR = "_";

        public delegate void TokensLoadedListener(AliensBag sender);

        private NetVariable<FixedString512Bytes> _contentNet;

        private AlienToken[] _content;

        public IReadOnlyCollection<AlienToken> AlienTokens => _content;

        private Dictionary<string, AsyncOperationHandle<AlienToken>> _loadedTokens = new();

        public NetVariable<bool> IsInitialized { get; private set; }

        public event TokensLoadedListener TokensLoaded;

        private void Awake()
        {
            _contentNet = new();
            IsInitialized = new();
        }

        private void OnEnable()
        {
            _contentNet.Changed += OnValueChange;
        }

        private void OnDisable()
        {
            _contentNet.Changed -= OnValueChange;
        }

        private void OnValueChange(FixedString512Bytes previousValue, FixedString512Bytes newValue)
        {
            string[] loadKeys = newValue.ToString().Split(SEPARATOR);

            List<AlienToken> result = new();

            int keysToLoadCount = loadKeys.Count();

            foreach (string loadKey in loadKeys)
            {
                AsyncOperationHandle<AlienToken> handle;
                if (_loadedTokens.ContainsKey(loadKey))
                {
                    handle = _loadedTokens[loadKey];
                }
                else
                {
                    AssetReferenceT<AlienToken> tokenReference = new(loadKey);
                    handle = tokenReference.LoadAssetAsync();
                    _loadedTokens.Add(loadKey, handle);
                }

                if (handle.IsDone)
                {
                    result.Add(handle.Result);
                    keysToLoadCount--;
                    if (keysToLoadCount <= 0)
                    {
                        _content = result.ToArray();
                        TokensLoaded?.Invoke(this);
                    }
                    continue;
                }

                handle.Completed += (loadedHandle) => 
                {
                    result.Add(loadedHandle.Result);
                    keysToLoadCount--;
                    if (keysToLoadCount <= 0)
                    {
                        _content = result.ToArray();
                        TokensLoaded?.Invoke(this);
                    }
                };
            }
        }

        public void Initialize(IEnumerable<AlienToken> tokens)
        {
            if (IsInitialized.Value)
            {
                throw new Exception("Aliens bug is already initialized");
            }

            if (NetworkManager.IsServer == false)
            {
                throw new NotServerException("Only server can initialize aliens bug");
            }

            _contentNet.Value = string.Join(SEPARATOR, tokens.Select(x => x.LoadKey)); 
            IsInitialized.Value = true;
        }

        public AlienToken PickRandom()
        {
            if (NetworkManager.IsServer == false)
            {
                throw new NotServerException("Only server can pick random alien token");
            }

            AlienToken randomToken = _content.ElementAt(UnityEngine.Random.Range(0, _content.Length));
            
            Regex regex = new Regex(Regex.Escape(randomToken.LoadKey));
            _contentNet.Value = regex.Replace(_contentNet.Value.ToString(), "", 1);

            return randomToken;
        }

        public void Add(AlienToken token)
        {
            if (NetworkManager.IsServer == false)
            {
                throw new NotServerException("Only server can add aliens tokens");
            }

            _contentNet.Value += SEPARATOR+token.LoadKey;
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(AliensBag))]
        private class CEditor : Editor
        {
            private AliensBag AliensBag => target as AliensBag;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (AliensBag.AlienTokens == null)
                {
                    return;
                }

                GUI.enabled = false;
                foreach (AlienToken alienToken in AliensBag.AlienTokens)
                {
                    EditorGUILayout.ObjectField(alienToken, typeof(AlienToken), false);
                }
                GUI.enabled = true;
            }
        }
#endif
    }
}