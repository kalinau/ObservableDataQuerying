using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Select.Immutable
{
    public abstract class SelectImmutableUpdate<TIn, TOut, TOperation> : IUpdate<TOperation>
    {
        [NotNull] private readonly IReadOnlyDictionary<TIn, ItemCounter<TOut>> _map;
        [CanBeNull] private readonly IReadOnlyDictionary<TIn, TOut> _removedItems;


        [CanBeNull] private IEnumerable<TOperation> _locked;
        private ThreadId _threadId;

        protected SelectImmutableUpdate(
            [NotNull] IReadOnlyDictionary<TIn, ItemCounter<TOut>> map,
            [CanBeNull] IReadOnlyDictionary<TIn, TOut> removedItems)
        {
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

        public IEnumerable<TOperation> Operations()
        {
            if (_locked != null) return _locked;
            _threadId.CheckIsCurrent();
            return this.Enumerate();
        }

        [NotNull]
        protected abstract IEnumerable<TOperation> Enumerate();

        protected TOut Select(TIn item)
        {
            ItemCounter<TOut> counter;
            TOut removed;

            if (_map.TryGetValue(item, out counter))
            {
                return counter.Item;
            }
            if (_removedItems != null && _removedItems.TryGetValue(item, out removed))
            {
                return removed;
            }
            throw new ArgumentOutOfRangeException();
        }

        protected TOut SelectRemoved(TIn item)
        {
            TOut removedItem;
            if (_removedItems != null && _removedItems.TryGetValue(item, out removedItem))
            {
                return removedItem;
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}