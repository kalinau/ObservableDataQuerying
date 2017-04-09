﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Select.Immutable
{
    public sealed class SelectImmutableCollectionUpdate<TIn, TOut> :  IUpdate<CollectionOperation<TOut>>
    {
        [NotNull] private readonly IUpdate<CollectionOperation<TIn>> _adaptee;
        [NotNull] private readonly IReadOnlyDictionary<TIn, ItemCounter<TOut>> _map;
        [CanBeNull] private readonly IReadOnlyDictionary<TIn, TOut> _removedItems;


        [CanBeNull] private IEnumerable<CollectionOperation<TOut>> _locked;
        private ThreadId _threadId;

        public SelectImmutableCollectionUpdate(
            [NotNull] IUpdate<CollectionOperation<TIn>> adaptee,
            [NotNull] IReadOnlyDictionary<TIn, ItemCounter<TOut>> map, 
            [CanBeNull] IReadOnlyDictionary<TIn, TOut> removedItems)
        {
            _adaptee = adaptee;
            _map = map;
            _removedItems = removedItems;
            _threadId = ThreadId.FromCurrent();
        }

        public void Lock()
        {
            if (_locked != null) return;
            _threadId.CheckIsCurrent();
            _locked = this.Enumerate().ToArray();
        }

        public IEnumerable<CollectionOperation<TOut>> Operations()
        {
            if (_locked != null) return _locked;
            _threadId.CheckIsCurrent();
            return this.Enumerate();
        }

        [NotNull]
        private IEnumerable<CollectionOperation<TOut>> Enumerate()
        {
            foreach (var update in _adaptee.Operations())
            {
                switch (update.Type)
                {
                    case CollectionOperationType.Add:
                        if (_map.TryGetValue(update.Item, out var counter))
                        {
                            yield return CollectionOperation<TOut>.OnAdd(counter.Item);
                        }
                        else if (_removedItems != null && _removedItems.TryGetValue(update.Item, out var addedItem))
                        {
                            yield return CollectionOperation<TOut>.OnAdd(addedItem);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        break;

                    case CollectionOperationType.Remove:
                        if (_removedItems != null && _removedItems.TryGetValue(update.Item, out var removedItem))
                        {
                            yield return CollectionOperation<TOut>.OnAdd(removedItem);
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }
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