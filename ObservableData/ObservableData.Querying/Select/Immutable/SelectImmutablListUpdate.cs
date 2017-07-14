using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;
using ObservableData.Structures;

namespace ObservableData.Querying.Select.Immutable
{
    public sealed class SelectImmutableListUpdate<TIn, TOut> : SelectImmutableUpdate<TIn, TOut, ListOperation<TOut>>
    {
        [NotNull] private readonly IUpdate<ListOperation<TIn>> _adaptee;

        public SelectImmutableListUpdate(
            [NotNull] IUpdate<ListOperation<TIn>> adaptee,
            [NotNull] IReadOnlyDictionary<TIn, ItemCounter<TOut>> map,
            [CanBeNull] IReadOnlyDictionary<TIn, TOut> removedItems)
            :base(map, removedItems)
        {
            _adaptee = adaptee;
        }

        protected override IEnumerable<ListOperation<TOut>> Enumerate()
        {
            foreach (var update in _adaptee.Operations())
            {
                switch (update.Type)
                {
                    case ListOperationType.Add:
                        yield return ListOperation<TOut>.OnAdd(this.Select(update.Item), update.Index);
                        break;

                    case ListOperationType.Remove:
                        yield return ListOperation<TOut>.OnRemove(this.SelectRemoved(update.Item), update.Index);
                        break;

                    case ListOperationType.Move:
                        yield return ListOperation<TOut>.OnMove(this.Select(update.Item), update.Index, update.OriginalIndex);
                        break;

                    case ListOperationType.Replace:
                        yield return ListOperation<TOut>.OnReplace(this.Select(update.Item), this.SelectRemoved(update.ChangedItem), update.Index);
                        break;

                    case ListOperationType.Clear:
                        yield return ListOperation<TOut>.OnClear();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}