using JetBrains.Annotations;

namespace ObservableData.Querying.Core
{
    public struct SetOperation<T>
    {
        private SetOperation(SetOperationType type, T item)
        {
            Item = item;
            Type = type;
        }

        public static SetOperation<T> OnClear()
        {
            return new SetOperation<T>(SetOperationType.Clear, default(T));
        }

        public static SetOperation<T> OnAdd(T item)
        {
            return new SetOperation<T>(SetOperationType.Add, item);
        }

        public static SetOperation<T> OnRemove(T item)
        {
            return new SetOperation<T>(SetOperationType.Remove, item);
        }

        public SetOperationType Type { get; }

        [CanBeNull]
        public T Item { get; }
    }
}