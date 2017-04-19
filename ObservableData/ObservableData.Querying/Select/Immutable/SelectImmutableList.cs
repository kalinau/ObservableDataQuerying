using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;
using ObservableData.Querying.Utils.Adapters;

namespace ObservableData.Querying.Select.Immutable
{
    public sealed class SelectImmutableList<TIn, TOut> : ListAdapter<TIn, TOut>
    {
        [NotNull] private readonly Dictionary<TIn, ItemCounter<TOut>> _map;

        public SelectImmutableList(
            [NotNull] IReadOnlyList<TIn> adaptee,
            [NotNull] Dictionary<TIn, ItemCounter<TOut>> map)
            :base(adaptee)
        {
            _map = map;
        }

        protected override IEnumerable<TOut> Enumerate(IEnumerable<TIn> source)
        {
            foreach (var item in source)
            {
                yield return this.Select(item);
            }
        }

        protected override TOut Select(TIn item)
        {
            ItemCounter<TOut> counter;
            if (!_map.TryGetValue(item, out counter))
            {
                throw new ArgumentOutOfRangeException();
            }
            return counter.Item;
        }
    }
}