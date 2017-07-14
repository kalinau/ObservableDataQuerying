using System;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters;
using ObservableData.Structures;

namespace ObservableData.Querying.Select.Constant
{
    internal sealed class SelectConstantCollectionObserver<TIn, TOut> : CollectionObserverAdapter<TIn, TOut>
    {
        [NotNull] private readonly Func<TIn, TOut> _select;

        public SelectConstantCollectionObserver([NotNull] IObserver<IUpdate<CollectionOperation<TOut>>> adaptee, [NotNull] Func<TIn, TOut> select) : base(adaptee)
        {
            _select = select;
        }

        protected override IUpdate<CollectionOperation<TOut>> HandleValue([NotNull] IUpdate<CollectionOperation<TIn>> value) =>
            new SelectConstantCollectionUpdate<TIn,TOut>(value, _select);
    }
}
