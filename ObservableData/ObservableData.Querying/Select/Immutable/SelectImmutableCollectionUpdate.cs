using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Select.Immutable
{
    public sealed class SelectImmutableCollectionUpdate<TIn, TOut> : SelectImmutableUpdate<TIn, TOut, CollectionOperation<TOut>>
    {
        [NotNull] private readonly IUpdate<CollectionOperation<TIn>> _adaptee;

        public SelectImmutableCollectionUpdate(
            [NotNull] IUpdate<CollectionOperation<TIn>> adaptee,
            [NotNull] IReadOnlyDictionary<TIn, ItemCounter<TOut>> map, 
            [CanBeNull] IReadOnlyDictionary<TIn, TOut> removedItems)
            :base(map, removedItems)
        {
            _adaptee = adaptee;
        }

        protected override IEnumerable<CollectionOperation<TOut>> Enumerate()
        {
            foreach (var update in _adaptee.Operations())
            {
                switch (update.Type)
                {

                    case CollectionOperationType.Add:
                        yield return CollectionOperation<TOut>.OnAdd(base.Select(update.Item));
                        break;

                    case CollectionOperationType.Remove:
                        yield return CollectionOperation<TOut>.OnRemove(base.SelectRemoved(update.Item));
                        break;

                    case CollectionOperationType.Clear:
                        yield return CollectionOperation<TOut>.OnClear();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}