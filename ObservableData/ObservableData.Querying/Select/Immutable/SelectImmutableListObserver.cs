using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;
using ObservableData.Querying.Utils.Adapters;

namespace ObservableData.Querying.Select.Immutable
{
    public sealed class SelectImmutableListObserver<TIn, TOut> : ListObserverAdapter<TIn, TOut>
    {
        [NotNull] private readonly Func<TIn, TOut> _func;
        [NotNull] private readonly Dictionary<TIn, ItemCounter<TOut>> _map;

        public SelectImmutableListObserver(
            [NotNull] IObserver<IUpdate<ListOperation<TOut>>> adaptee,
            [NotNull] Func<TIn, TOut> func,
            [NotNull] Dictionary<TIn, ItemCounter<TOut>> map)
            : base(adaptee)
        {
            _func = func;
            _map = map;
        }

        protected override IUpdate<ListOperation<TOut>> HandleValue([NotNull] IUpdate<ListOperation<TIn>> e)
        {
            Dictionary<TIn, TOut> removedMap = null;
            foreach (var update in e.Operations())
            {
                switch (update.Type)
                {
                    case ListOperationType.Add:
                        this.OnAdd(update, removedMap);
                        break;

                    case ListOperationType.Remove:
                        this.OnRemove(update, ref removedMap);
                        break;

                    case ListOperationType.Move:
                        break;

                    case ListOperationType.Replace:
                        this.OnRemove(update, ref removedMap);
                        this.OnAdd(update, removedMap);
                        break;

                    case ListOperationType.Clear:
                        _map.Clear();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return new SelectImmutableListUpdate<TIn, TOut>(e, _map, removedMap);
        }

        private void OnRemove(ListOperation<TIn> update, ref Dictionary<TIn, TOut> removedMap)
        {
            TOut removed;
            if (_map.DecreaseCount(update.Item, out removed) <= 1)
            {
                if (removedMap == null)
                {
                    removedMap = new Dictionary<TIn, TOut>();
                }
                removedMap[update.Item] = removed;
            }
        }

        private void OnAdd(ListOperation<TIn> update, Dictionary<TIn, TOut> removedMap)
        {
            if (!_map.TryIncreaseCount(update.Item))
            {
                var result = default(TOut);
                if (removedMap?.TryGetValue(update.Item, out result) != true)
                {
                    result = _func(update.Item);
                }
                _map[update.Item] = new ItemCounter<TOut>(result, 1);
            }
        }
    }
}