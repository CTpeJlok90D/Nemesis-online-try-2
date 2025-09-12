using System.Collections.Generic;
using System.Linq;
using Core.Maps.CharacterPawns;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Core.Characters.Health
{
    public class CharacterHealth : NetworkBehaviour
    {
        private const int LIGHTS_TO_HEAVY = 3;
        private const int HEAVYS_TO_DEATH = 3;
        
        private NetVariable<int> _lightDamageCount;
        private NetworkList<NetworkObjectReference> _heavyDamages;
        private CharacterPawn _characterPawn;
        
        public IReadOnlyReactiveField<int> LightDamageCount => _lightDamageCount;
        public IEnumerable<HeavyDamage> HeavyDamages => _heavyDamages.ToEnumerable<HeavyDamage>();
        
        [Inject] private HeavyDamageDeck _heavyDamageDeck;

        public delegate void DealthHandler(CharacterHealth characterHealth);
        public delegate void HeavyDamageTreadedHandler(HeavyDamage heavyDamage);
        public event DealthHandler Dead;
        public event HeavyDamageTreadedHandler HeavyDamageTreaded;
        public event NetworkList<NetworkObjectReference>.OnListChangedDelegate HeavyDamagesCountChanged
        {
            add => _heavyDamages.OnListChanged += value;
            remove => _heavyDamages.OnListChanged -= value;
        }
        
        private void Awake()
        {
            _characterPawn = GetComponent<CharacterPawn>();
            _lightDamageCount = new();
            _heavyDamages = new();
        }

        public void LightDamage(int count = 1)
        {
            _lightDamageCount.Value += count;
            int overdamage =  _lightDamageCount.Value / LIGHTS_TO_HEAVY;
            _lightDamageCount.Value %= LIGHTS_TO_HEAVY;

            if (overdamage > 0)
            {
                HeavyDamage(overdamage);
                return;
            }

            TryKill();
        }

        public void HeavyDamage(int count = 1)
        {
            HeavyDamage[] damages = _heavyDamageDeck.Pick(count).ToArray();
            HeavyDamage(damages);
        }

        public async UniTask HeavyDamage(params string[] damageIds)
        {
            foreach (string id in damageIds)
            {
                await HeavyDamage(id);
            }
        }

        public async UniTask HeavyDamage(string damageId)
        {
            AsyncOperationHandle<HeavyDamage> handle = Addressables.LoadAssetAsync<HeavyDamage>(damageId);

            if (handle.IsValid())
            {
                throw new System.ArgumentException($"Damage id {damageId} is invalid", nameof(damageId));
            }

            await handle.ToUniTask();
            HeavyDamage heavyDamage = handle.Result;
            HeavyDamage(heavyDamage);
        }

        public void HeavyDamage(params HeavyDamage[] damagePrefabs)
        {
            foreach (HeavyDamage damage in damagePrefabs)
            {
                if (damage.IsInitialized)
                {
                    Debug.LogError($"Damage {damage} is already initialized", damage);
                    continue;
                }

                HeavyDamage(damage);
            }
        }

        public void HeavyDamage(HeavyDamage damagePrefab)
        {
            if (damagePrefab == null)
            {
                throw new System.ArgumentNullException(nameof(damagePrefab));
            }

            HeavyDamage instance = damagePrefab.Instantiate(this);
            _heavyDamages.Add(instance.NetworkObject);
            
            TryKill();
        }

        public void TreadHeavyDamage(HeavyDamage damage)
        {
            if (damage == null)
            {
                throw new System.ArgumentNullException(nameof(damage));
            }

            damage.Tread();
            HeavyDamageTreaded?.Invoke(damage);
        }

        public void RemoveHeavyDamage(HeavyDamage damage)
        {
            if (damage == null)
            {
                throw new System.ArgumentNullException(nameof(damage));
            }

            _heavyDamages.Remove(damage.NetworkObject);
            damage.NetworkObject.Despawn();
        }

        private bool TryKill()
        {
            if (_heavyDamages.Count == HEAVYS_TO_DEATH && LightDamageCount.Value > 0 || _heavyDamages.Count > HEAVYS_TO_DEATH)
            {
                ForceKill();
                return true;
            }
            return false;
        }

        public void ForceKill()
        {
            PlayerTablet tablet = PlayerTablet.Instances.First(x => x.CharacterPawn == _characterPawn);
            tablet.ForceKill();
            Dead?.Invoke(this);
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(CharacterHealth))]
        private class CEditor : Editor
        {
            private CharacterHealth CharacterHealth => target as CharacterHealth;
            
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (Application.IsPlaying(target) == false)
                {
                    EditorGUILayout.HelpBox("This UI works in play mode only", MessageType.Info);
                    return;
                }
                
                GUILayout.Space(10);
                string lightDamageCount = string.Concat(Enumerable.Repeat("\ud83d\udd34", CharacterHealth.LightDamageCount.Value));
                GUILayout.Label($"Light Damage: {lightDamageCount}");

                GUI.enabled = false;
                foreach (HeavyDamage heavyDamage in CharacterHealth.HeavyDamages)
                {
                    EditorGUILayout.ObjectField("", heavyDamage, typeof(HeavyDamage), false);
                }
                GUI.enabled = true;
            }
        }
#endif
    }
}