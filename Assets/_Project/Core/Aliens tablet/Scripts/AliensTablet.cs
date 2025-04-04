using System;
using Core.Aliens;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEditor;
using UnityEngine;

namespace Core.AliensTablets
{
    [Icon("Assets/_Project/Core/Aliens tablet/Editor/icons8-hive-96.png")]
    public class AliensTablet : NetworkBehaviour
    {
        private const string SEPARATOR = "_";
        
        public NetVariable<int> EggCount { get; private set; }

        private NetVariable<bool> _isInitialized;

        private NetScriptableObjectList4096<AlienWeaknessCard> _alienWeaknessCards;

        private void Awake()
        {
            _alienWeaknessCards = new();
            _isInitialized = new();
            EggCount = new();
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

            _alienWeaknessCards.AddRange(weaknessCards);
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
