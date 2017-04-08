using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Core;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Select.Immutable
{
    public sealed class SelectImmutableSetUpdate<TIn, TOut> : MutableUpdate, IUpdate<SetOperation<TOut>>
    {
        [NotNull] private readonly IUpdate<SetOperation<TIn>> _adaptee;
        [NotNull] private readonly IReadOnlyDictionary<TIn, ItemCounter<TOut>> _map;
        [CanBeNull] private readonly IReadOnlyDictionary<TIn, TOut> _removedItems;


        [CanBeNull] private IEnumerable<SetOperation<TOut>> _locked;

        public SelectImmutableSetUpdate(
            [NotNull] IUpdate<SetOperation<TIn>> adaptee,
            [NotNull] IReadOnlyDictionary<TIn, ItemCounter<TOut>> map, 
            [CanBeNull] IReadOnlyDictionary<TIn, TOut> removedItems)
        {
            _adaptee = adaptee;
            _map = map;
            _removedItems = removedItems;
        }

        public void Lock()
        {
            if (_locked != null) return;
            base.CheckAccess();
            _locked = this.Enumerate().ToArray();
        }

        public IEnumerable<SetOperation<TOut>> Operations()
        {
            if (_locked != null) return _locked;
            base.CheckAccess();
            return this.Enumerate();
        }

        [NotNull]
        private IEnumerable<SetOperation<TOut>> Enumerate()
        {
            foreach (var update in _adaptee.Operations())
            {
                switch (update.Type)
                {
                    case SetOperationType.Add:
                        if (_map.TryGetValue(update.Item, out var counter))
                        {
                            yield return SetOperation<TOut>.OnAdd(counter.Item);
                        }
                        else if (_removedItems != null && _removedItems.TryGetValue(update.Item, out var addedItem))
                        {
                            yield return SetOperation<TOut>.OnAdd(addedItem);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        break;

                    case SetOperationType.Remove:
                        if (_removedItems != null && _removedItems.TryGetValue(update.Item, out var removedItem))
                        {
                            yield return SetOperation<TOut>.OnAdd(removedItem);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        break;
                    case SetOperationType.Clear:
                        yield return SetOperation<TOut>.OnClear();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}