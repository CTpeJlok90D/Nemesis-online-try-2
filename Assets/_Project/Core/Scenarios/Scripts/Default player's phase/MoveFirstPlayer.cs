namespace Core.Scenarios.PlayersPhase
{
    public class MoveFirstPlayer : IChapter
    {
        private ActionPointsGiver _actionPointsGiver;
        public MoveFirstPlayer(ActionPointsGiver actionPointsGiver)
        {
            _actionPointsGiver = actionPointsGiver;
        }

        public event IChapter.EndedListener Ended;

        public void Begin()
        {
            _actionPointsGiver.MoveFirstPlayer();
            Ended?.Invoke(this);
        }
    }
}
