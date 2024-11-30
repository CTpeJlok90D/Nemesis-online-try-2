using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.AliensTablets
{
    [Icon("Assets/_Project/Core/Aliens tablet/Editor/icons8-hive-96.png")]
    public class AliensTablet : NetworkBehaviour
    {
        private string SEPARATOR = "_";
        public NetVariable<int> EggCount { get; private set; }

        private NetVariable<FixedString128Bytes> _alienWeaknessesNet;

        private List<AlienWeaknessCard> _alienWeaknessCards;

        private NetVariable<bool> _isInitialized;

        private Dictionary<string, AlienWeaknessCard> _loadedCards = new();

        private void Awake()
        {
            EggCount = new();
            _alienWeaknessesNet = new();
            _alienWeaknessCards = new();
            _isInitialized = new();
            _alienWeaknessesNet.Changed += OnListChange;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _alienWeaknessesNet.Changed -= OnListChange;
        }

        private void OnListChange(FixedString128Bytes previousValue, FixedString128Bytes newValue)
        {
            string[] keys = _alienWeaknessesNet.Value.ToString().Split(SEPARATOR);
            _alienWeaknessCards = new();

            foreach (string key in keys)
            {
                if (_loadedCards.ContainsKey(key))
                {
                    _alienWeaknessCards.Add(_loadedCards[key]);
                }
                else
                {
                    AssetReferenceT<AlienWeaknessCard> reference = new AssetReferenceT<AlienWeaknessCard>(key);
                    AsyncOperationHandle<AlienWeaknessCard> asyncOperationhande = reference.LoadAssetAsync();
                    asyncOperationhande.Completed += (handle) => 
                    {
                        _alienWeaknessCards.Add(handle.Result);

                        if (_loadedCards.ContainsKey(key) == false)
                        {
                            _loadedCards.Add(key, handle.Result);
                        }
                    };
                }
            }
        }

        public void Initialize(AlienWeaknessCard[] weaknessCards)
        {
            if (_isInitialized.Value)
            {
                throw new AlienTabletAlreadyInitialized();
            }

            if (NetworkManager.IsServer == false)
            {
                throw new NotServerException("Only server can initialize aliens tablet");
            }

            _alienWeaknessesNet.Value = new(string.Join(SEPARATOR, weaknessCards.Select(x => x.LoadAssetReference.RuntimeKey)));
            _isInitialized.Value = true;
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(AliensTablet))]
        private class CEditor : Editor
        {
            private AliensTablet AliensTablet => target as AliensTablet;
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (AliensTablet._alienWeaknessCards != null)
                {
                    GUI.enabled = false;
                    GUILayout.Label("Alien weakness cards");
                    foreach (AlienWeaknessCard card in AliensTablet._alienWeaknessCards)
                    {
                        EditorGUILayout.ObjectField(card, typeof(AlienWeaknessCard), false);
                    }
                    GUI.enabled = true;
                }
            }
        }
#endif
    }
}
