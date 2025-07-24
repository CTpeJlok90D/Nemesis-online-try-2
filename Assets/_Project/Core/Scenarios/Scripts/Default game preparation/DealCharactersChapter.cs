using System.Threading.Tasks;
using Core.CharacterChoose;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Scenarios
{
    public class DealCharactersChapter : IChapter
    {
        private CharactersDealer _charactersDealer;

        public DealCharactersChapter(CharactersDealer charactersDealer)
        {
            _charactersDealer = charactersDealer;
        }

        public event IChapter.EndedListener Ended;

        public void Begin()
        {
            _ = DealAsync();
        }

        private async UniTask DealAsync()
        {
            await _charactersDealer.StartDeal();
            Ended?.Invoke(this);
        }
    }
}
