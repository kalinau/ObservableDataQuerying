namespace ObservableData.Querying.Utils
{
    public struct ItemCounter<T>
    {
        private readonly uint _count;
        private readonly T _item;

        public ItemCounter(T item, uint count)
        {
            _count = count;
            _item = item;
        }

        public T Item => _item;

        public uint Count => _count;
    }
}
