using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters;

namespace ObservableData.Querying.Select.Constant
{
    internal sealed class SelectConstantCollection<TIn, TOut> : CollectionAdapter<TIn, TOut>
    {
        [NotNull] private readonly Func<TIn, TOut> _select;

        public SelectConstantCollection([NotNull] IReadOnlyCollection<TIn> source, [NotNull] Func<TIn, TOut> select) : base(source)
        {
            _select = @select;
        }

        protected override IEnumerable<TOut> Enumerate(IEnumerable<TIn> source) => source.Select(_select);
    }
}
