using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Select.Immutable
{
    public sealed class SelectImmutableCollection<TIn, TOut> : IReadOnlyCollection<TOut>
    {
        [NotNull] private readonly IReadOnlyCollection<TIn> _adaptee;
        [NotNull] private readonly Dictionary<TIn, ItemCounter<TOut>> _map;

        public SelectImmutableCollection(
            [NotNull] IReadOnlyCollection<TIn> adaptee,
            [NotNull] Dictionary<TIn, ItemCounter<TOut>> map)
        {
            _adaptee = adaptee;
            _map = map;
        }

        [NotNull]
        private IEnumerable<TOut> Enumerate()
        {
            Debug.Assert(_map.Values != null);
            return _map.Values.Enumerate();
        }

        public IEnumerator<TOut> GetEnumerator() => this.Enumerate().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public int Count => _adaptee.Count;
    }
}