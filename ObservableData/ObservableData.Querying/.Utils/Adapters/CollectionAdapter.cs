using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils.Adapters
{
    public abstract class CollectionAdapter<TIn, TOut> : IReadOnlyCollection<TOut>
    {
        [NotNull] private readonly IReadOnlyCollection<TIn> _source;

        protected CollectionAdapter([NotNull] IReadOnlyCollection<TIn> source)
        {
            _source = source;
        }

        public int Count => _source.Count;

        public IEnumerator<TOut> GetEnumerator() => this.Enumerate(_source).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        [NotNull]
        protected abstract IEnumerable<TOut> Enumerate(IEnumerable<TIn> source);
    }
}