using System;
using System.Collections.Generic;
using System.Linq;
using Core.Aliens;
using Core.Entities;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEditor;
using UnityEngine;

namespace Core.AliensTablets
{
    [Icon("Assets/_Project/Core/Aliens tablet/Editor/icons8-hive-96.png")]
    public class AliensTablet : NetEntity<AliensTablet>
    {
        public NetVariable<int> EggCount { get; private set; }
        private NetVariable<bool> _isInitialized;
        private NetScriptableObjectList4096<AlienWeaknessCard> _alienWeaknessCards;
        private NetworkList<int> _unlockedWeaknesses;

        public IReadOnlyCollection<AlienWeaknessType> UnlockedWeaknessTypes =>
            _unlockedWeaknesses.ToEnumerable().Cast<AlienWeaknessType>().ToArray();

        public IReadOnlyCollection<AlienWeaknessCard> UnlockedWeaknessesCards =>
            _alienWeaknessCards.CashedElements;

        private void Awake()
        {
            _alienWeaknessCards = new();
            _isInitialized = new();
            EggCount = new();
            _unlockedWeaknesses = new();
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

            _alienWeaknessCards.AddRange(weaknessCards.Select(x => x.Instantiate()));
        }

        public void UnlockWeakness(AlienWeaknessType alienWeaknessType)
        {
            int index = (int)alienWeaknessType;
            
            _unlockedWeaknesses.Add(index);
            AlienWeaknessCard card = _alienWeaknessCards[index];
            card.Weakness.IsActive = true;
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
