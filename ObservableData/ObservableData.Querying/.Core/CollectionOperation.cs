﻿using JetBrains.Annotations;

namespace ObservableData.Querying.Core
{
    public struct CollectionOperation<T>
    {
        private CollectionOperation(CollectionOperationType type, T item)
        {
            Item = item;
            Type = type;
        }

        public static CollectionOperation<T> OnClear()
        {
            return new CollectionOperation<T>(CollectionOperationType.Clear, default(T));
        }

        public static CollectionOperation<T> OnAdd(T item)
        {
            return new CollectionOperation<T>(CollectionOperationType.Add, item);
        }

        public static CollectionOperation<T> OnRemove(T item)
        {
            return new CollectionOperation<T>(CollectionOperationType.Remove, item);
        }

        public CollectionOperationType Type { get; }

        [CanBeNull]
        public T Item { get; }
    }
}