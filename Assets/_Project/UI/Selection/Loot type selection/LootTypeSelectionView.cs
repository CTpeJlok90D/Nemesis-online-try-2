using System.Linq;
using Core.Entities;
using Core.LootDecks;
using Core.Selection.LootDeckSelections;
using UnityEngine;
using Zenject;

namespace UI.Selection.LootTypeSelection
{
    public class LootTypeSelectionView : MonoBehaviour
    {
        [SerializeField] private LootDeck.Type _linkedDeckType;
        [SerializeField] private GameObject _selectedView;
        [Inject] private LootDeckSelection _lootDeckSelection;
        
        private LootDeck _lootDeck;
        
        private void Awake()
        {
            _lootDeck = NetEntity<LootDeck>.Instances.First(x => x.DeckType == _linkedDeckType);
        }
        
        private void Update()
        {
            _selectedView.SetActive(_lootDeckSelection.Contains(_lootDeck.DeckType));
        }
    }
}
