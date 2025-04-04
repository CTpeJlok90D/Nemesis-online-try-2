using System.Collections.Generic;
using System.Linq;
using Core.Aliens;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEditor;
using UnityEngine;

namespace Core.AliensBags
{
    public class AliensBag : NetworkBehaviour
    {
        [SerializeField] private AlienToken _emptyToken;
        
        private NetScriptableObjectList4096<AlienToken> _contentNet;
        public async UniTask<IReadOnlyCollection<AlienToken>> GetAlienTokens() => await _contentNet.GetElements();

        private void Awake()
        {
            _contentNet = new();
        }
        
        public void Initialize(IEnumerable<AlienToken> tokens)
        {
            if (NetworkManager.IsServer == false)
            {
                throw new NotServerException("Only server can initialize aliens bug");
            }

            _contentNet.SetElements(tokens); 
        }

        /// <summary>
        /// Returns random AlienToken and remove it from bag;
        /// </summary>
        public AlienToken PickRandom()
        {
            if (NetworkManager.IsServer == false)
            {
                throw new NotServerException("Only server can pick random alien token");
            }

            AlienToken randomToken = _contentNet.ElementAt(Random.Range(0, _contentNet.Count));
            if (randomToken.IsEmpty == false)
            {
                _contentNet.Remove(randomToken);
            }

            return randomToken;
        }

        public void Add(AlienToken token)
        {
            if (NetworkManager.IsServer == false)
            {
                throw new NotServerException("Only server can add aliens tokens");
            }

            _contentNet.Add(token);
        }

        public bool Remove(AlienToken token)
        {
            if (NetworkManager.IsServer == false)
            {
                throw new NotServerException("Only server can remove aliens tokens");
            }
            
            return _contentNet.Remove(token);
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(AliensBag))]
        private class CEditor : Editor
        {
            private AliensBag AliensBag => target as AliensBag;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                
                GUI.enabled = false;
                foreach (AlienToken alienToken in AliensBag._contentNet.CashedElements)
                {
                    EditorGUILayout.ObjectField(alienToken, typeof(AlienToken), false);
                }
                GUI.enabled = true;
            }
        }
#endif
    }
}