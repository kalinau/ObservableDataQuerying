using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters.ToCollection;

namespace ObservableData.Querying.Where.Immutable
{
    public sealed class WhereByImmutableCollection<T>: CollectionAdapter<T>
    {
        [NotNull] private readonly Func<T, bool> _func;

        public WhereByImmutableCollection(
            [NotNull] IReadOnlyCollection<T> source,
            [NotNull] Func<T, bool> func)
            :base(source)
        {
            _func = func;
        }

        protected override IEnumerable<T> Enumerate(IEnumerable<T> source) => source.Where(_func);
    }
}
