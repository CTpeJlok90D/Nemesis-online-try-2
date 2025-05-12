using System.Linq;
using Core.Entities;
using Core.LootDecks;
using Core.Selection.LootDeckSelections;
using UnityEngine;
using Zenject;

namespace UI.Selection.LootTypeSelection
{
    public class LootTypeIsAvailableItem : MonoBehaviour
    {
        [SerializeField] private GameObject _target;
        [SerializeField] private LootDeck.Type _deckType;

        [Inject] private LootDeckSelection _selection;
        
        private LootDeck _lootDeck;

        private void Awake()
        {
            _lootDeck = NetEntity<LootDeck>.Instances.First(x => x.DeckType == _deckType);
        }

        private void Update()
        {
            _target.SetActive(_selection.SelectionSource.Contains(_lootDeck.DeckType));
        }
    }
}
