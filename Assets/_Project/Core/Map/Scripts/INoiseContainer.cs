namespace Core.Maps
{
    public interface INoiseContainer
    {
        public bool IsNoised { get; }

        public void Noise();

        public void Clear();
    }
}
