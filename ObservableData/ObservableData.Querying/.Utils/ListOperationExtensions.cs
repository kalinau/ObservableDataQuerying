using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Core;

namespace ObservableData.Querying.Utils
{
    public static class ListOperationExtensions
    {
        private sealed class ListUpdateAdapter<T> : IUpdate<SetOperation<T>>
        {
            [NotNull] private readonly IUpdate<ListOperation<T>> _adaptee;

            public ListUpdateAdapter([NotNull] IUpdate<ListOperation<T>> adaptee)
            {
                _adaptee = adaptee;
            }

            public IEnumerable<SetOperation<T>> Operations() => _adaptee.Operations().AsSetOperations();

            public void Lock() => _adaptee.Lock();
        }


        [NotNull]
        public static IUpdate<SetOperation<T>> AsSetUpdate<T>([NotNull] this IUpdate<ListOperation<T>> update) =>
            new ListUpdateAdapter<T>(update);

        [NotNull]
        public static IEnumerable<SetOperation<T>> AsSetOperations<T>([NotNull] this IEnumerable<ListOperation<T>> operations)
        {
            foreach (var update in operations)
            {
                switch (update.Type)
                {
                    case ListOperationType.Add:
                        yield return SetOperation<T>.OnAdd(update.Item);
                        break;

                    case ListOperationType.Remove:
                        yield return SetOperation<T>.OnRemove(update.Item);
                        break;

                    case ListOperationType.Move:
                        break;

                    case ListOperationType.Replace:
                        yield return SetOperation<T>.OnRemove(update.ChangedItem);
                        yield return SetOperation<T>.OnAdd(update.Item);
                        break;

                    case ListOperationType.Clear:
                        yield return SetOperation<T>.OnClear();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
