﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils
{
    public static class ListOperationExtensions
    {
        private sealed class ListUpdateAdapter<T> : IUpdate<CollectionOperation<T>>
        {
            [NotNull] private readonly IUpdate<ListOperation<T>> _adaptee;

            public ListUpdateAdapter([NotNull] IUpdate<ListOperation<T>> adaptee)
            {
                _adaptee = adaptee;
            }

            public IEnumerable<CollectionOperation<T>> Operations() => _adaptee.Operations().AsCollectionOperations();

            public void Lock() => _adaptee.Lock();
        }


        [NotNull]
        public static IUpdate<CollectionOperation<T>> AsCollectionUpdate<T>([NotNull] this IUpdate<ListOperation<T>> update) =>
            new ListUpdateAdapter<T>(update);

        [NotNull]
        public static IEnumerable<CollectionOperation<T>> AsCollectionOperations<T>([NotNull] this IEnumerable<ListOperation<T>> operations)
        {
            foreach (var update in operations)
            {
                switch (update.Type)
                {
                    case ListOperationType.Add:
                        yield return CollectionOperation<T>.OnAdd(update.Item);
                        break;

                    case ListOperationType.Remove:
                        yield return CollectionOperation<T>.OnRemove(update.Item);
                        break;

                    case ListOperationType.Move:
                        break;

                    case ListOperationType.Replace:
                        yield return CollectionOperation<T>.OnRemove(update.ChangedItem);
                        yield return CollectionOperation<T>.OnAdd(update.Item);
                        break;

                    case ListOperationType.Clear:
                        yield return CollectionOperation<T>.OnClear();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}