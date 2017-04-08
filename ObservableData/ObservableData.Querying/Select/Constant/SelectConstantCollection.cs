using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters.ToCollection;

namespace ObservableData.Querying.Select.Constant
{
    public sealed class SelectConstantCollection<TIn, TOut> : LazyCollectionAdapter<TIn, TOut>
    {
        [NotNull] private readonly Func<TIn, TOut> _func;

        public SelectConstantCollection(
            [NotNull] IReadOnlyCollection<TIn> source,
            [NotNull] Func<TIn, TOut> func)
            :base(source)
        {
            _func = func;
        }

        protected override IEnumerable<TOut> Enumerate(IEnumerable<TIn> source) => source.Select(_func);
    }
}
