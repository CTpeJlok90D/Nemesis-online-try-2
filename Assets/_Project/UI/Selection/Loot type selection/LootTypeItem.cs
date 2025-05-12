using System.Linq;
using Core.Entities;
using Core.LootDecks;
using Core.Selection.LootDeckSelections;
using UI.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.Selection.LootTypeSelection
{
    [DefaultExecutionOrder(5)]
    [RequireComponent(typeof(PointerEvents))]
    public class LootTypeItem : MonoBehaviour
    {
        [SerializeField] private LootDeck.Type _linkedDeckType;
        [Inject] private LootDeckSelection _lootDeckSelection;
        
        private PointerEvents _pointerEvents;
        private LootDeck _lootDeck;
        
        private void Awake()
        {
            _pointerEvents = GetComponent<PointerEvents>();
            _lootDeck = NetEntity<LootDeck>.Instances.First(x => x.DeckType == _linkedDeckType);
        }
        
        private void OnEnable()
        {
            _pointerEvents.PointerClicked += OnClick;
        }

        private void OnDisable()
        {
            _pointerEvents.PointerClicked -= OnClick;
        }
        
        private void OnClick(PointerEvents sender, PointerEventData eventData)
        {
            if (_lootDeckSelection.Contains(_lootDeck.DeckType))
            {
                _lootDeckSelection.Remove(_lootDeck.DeckType);
            }
            else
            {
                _lootDeckSelection.Add(_lootDeck.DeckType);
            }
        }
    }
}
