using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;
using ObservableData.Querying.Utils.Adapters;
using ObservableData.Structures;

namespace ObservableData.Querying.Select.Immutable
{
    public sealed class SelectImmutableCollectionObserver<TIn, TOut> : CollectionObserverAdapter<TIn, TOut>
    {
        [NotNull] private readonly Func<TIn, TOut> _func;
        [NotNull] private readonly Dictionary<TIn, ItemCounter<TOut>> _map;

        public SelectImmutableCollectionObserver(
            [NotNull] IObserver<IUpdate<CollectionOperation<TOut>>> adaptee,
            [NotNull] Func<TIn, TOut> func,
            [NotNull] Dictionary<TIn, ItemCounter<TOut>> map)
            : base(adaptee)
        {
            _func = func;
            _map = map;
        }

        protected override IUpdate<CollectionOperation<TOut>> HandleValue([NotNull] IUpdate<CollectionOperation<TIn>> e)
        {
            Dictionary<TIn, TOut> removedMap = null;
            foreach (var update in e.Operations())
            {
                switch (update.Type)
                {
                    case CollectionOperationType.Add:
                        if (!_map.TryIncreaseCount(update.Item))
                        {
                            var result = default(TOut);
                            if (removedMap?.TryGetValue(update.Item, out result) != true)
                            {
                                result = _func(update.Item);
                            }
                            _map[update.Item] = new ItemCounter<TOut>(result, 1);
                        }
                        break;

                    case CollectionOperationType.Remove:
                        TOut removed;
                        if (_map.DecreaseCount(update.Item, out removed) <= 1)
                        {
                            if (removedMap == null)
                            {
                                removedMap = new Dictionary<TIn, TOut>();
                            }
                            removedMap[update.Item] = removed;
                        }
                        break;

                    case CollectionOperationType.Clear:
                        _map.Clear();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return new SelectImmutableCollectionUpdate<TIn, TOut>(e, _map, removedMap);
        }
    }
}
