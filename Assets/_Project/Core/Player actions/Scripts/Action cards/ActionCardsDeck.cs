using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Custom;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Core.ActionsCards
{
    [RequireComponent(typeof(NetworkObject))]
    public class ActionCardsDeck : NetworkBehaviour
    {
        private NetScriptableObjectList4096<ActionCard> _mainDeck;

        private NetScriptableObjectList4096<ActionCard> _discard;

        private NetScriptableObjectList4096<ActionCard> _hand;

        [Inject] private Config _config;

        public IReadOnlyCollection<ActionCard> HandLocal => _hand.ToArray();
        public async UniTask<IReadOnlyCollection<ActionCard>> GetHand() => await _hand.GetElements();
        public async UniTask<IReadOnlyCollection<ActionCard>> GetDiscard() => await _discard.GetElements();
        public async UniTask<IReadOnlyCollection<ActionCard>> GetMainDeck() => await _mainDeck.GetElements();

        public event NetScriptableObjectList4096<ActionCard>.ListChangedListener HandChanged
        {
            add { _hand.ListChanged += value; }
            remove { _hand.ListChanged -= value; }
        }

        public event NetScriptableObjectList4096<ActionCard>.ListChangedListener DiscardChanged
        {
            add { _discard.ListChanged += value; }
            remove { _discard.ListChanged -= value; }
        }

        public event NetScriptableObjectList4096<ActionCard>.ListChangedListener MainChanged
        {
            add { _mainDeck.ListChanged += value; }
            remove { _mainDeck.ListChanged -= value; }
        }

        private void Awake()
        {
            _mainDeck = new();
            _discard = new();
            _hand = new();
        }

        public void InitializeDeck(IEnumerable<ActionCard> actionCards)
        {
            _mainDeck.SetElements(actionCards.ToArray());
            _discard.Clear();
            _hand.Clear();
            _ = ShuffleActionDeck();
        }

        private void OnListSync(NetScriptableObjectList4096<ActionCard> sender)
        {
            _mainDeck.ListChanged -= OnListSync;
            _ = ShuffleActionDeck();
        }

        public void DiscardCards(IEnumerable<ActionCard> actionCards)
        {
            foreach (ActionCard card in actionCards)
            {
                DiscardCard(card);
            }
        }

        public void DiscardCard(ActionCard actionCard)
        {
            if (_hand.Remove(actionCard))
            {
                _discard.Add(actionCard);
            }
        }

        public void AddCardToDiscard(ActionCard actionCard)
        {
            _discard.Add(actionCard);
        }

        public async UniTask<IReadOnlyCollection<ActionCard>> DrawCards(int count = -1)
        {
            if (count == -1)
            {
                count = _config.MaxHandSize - _hand.Count;
            }

            if (_mainDeck.Count < count)
            {
                await ShuffleActionDeck();
            }

            ActionCard[] mainDeckCards = await _mainDeck.GetElements();
            ActionCard[] cards = mainDeckCards.Take(count).ToArray();
            _mainDeck.RemoveRange(cards);
            _hand.AddRange(cards);
            return cards;
        }

        public async Task ShuffleActionDeck()
        {
            try
            {
                _mainDeck.AddRange(_discard);
                _discard.Clear();

                ActionCard[] actionCards = await _mainDeck.GetElements();
                _mainDeck.SetElements(actionCards.OrderBy(x => UnityEngine.Random.value).ToArray());
                await _mainDeck.Sync();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        [Serializable]
        public struct Config : INetworkSerializable
        {
            public int MaxHandSize;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref MaxHandSize);
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(ActionCardsDeck))]
        private class CEditor : Editor
        {
            private ActionCardsDeck ActionCardsDeck => target as ActionCardsDeck;

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (Application.isPlaying == false)
                {
                    EditorGUILayout.HelpBox("This UI works in play mode only", MessageType.Info);
                    return;
                }

                GUI.enabled = false;
                GUILayout.Label($"Hand: {ActionCardsDeck._hand.Count}");
                DisplayCards(ActionCardsDeck._hand);
                GUILayout.Label($"Main deck: {ActionCardsDeck._mainDeck.Count}");
                DisplayCards(ActionCardsDeck._mainDeck);
                GUILayout.Label($"Discard: {ActionCardsDeck._discard.Count}");
                DisplayCards(ActionCardsDeck._discard);
                GUI.enabled = true;

                if (GUILayout.Button("Remove card"))
                {
                    ActionCard actionCard = ActionCardsDeck._hand.ElementAt(0);
                    ActionCardsDeck.DiscardCard(actionCard);
                }

                if (GUILayout.Button("Draw card"))
                {
                    _ = ActionCardsDeck.DrawCards(1);
                }
            }

            private void DisplayCards(IEnumerable<ActionCard> cards)
            {
                foreach (ActionCard actionCard in cards)
                {
                    EditorGUILayout.ObjectField(actionCard, typeof(ActionCard), false);
                }
            }
        }
#endif
    }
}
