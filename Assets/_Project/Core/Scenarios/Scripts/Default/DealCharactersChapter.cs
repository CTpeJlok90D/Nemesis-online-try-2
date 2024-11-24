using Core.CharacterChoose;

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
            DealAsync();
        }

        private async void DealAsync()
        {
            await _charactersDealer.StartDeal();
            Ended?.Invoke(this);
        }
    }
}
