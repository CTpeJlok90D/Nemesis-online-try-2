namespace Core
{
    public interface IReadOnlyReactiveField<T>
    {
        public T Value { get; }

        public delegate void ChangedListener(T oldValue, T newValue);
        public event ChangedListener Changed;
    }
}
