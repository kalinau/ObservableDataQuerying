using System;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters;

namespace ObservableData.Querying.Select.Constant
{
    internal sealed class SelectConstantListObserver<TIn, TOut> : ListObserverAdapter<TIn, TOut>
    {
        [NotNull] private readonly Func<TIn, TOut> _select;

        public SelectConstantListObserver([NotNull] IObserver<IUpdate<ListOperation<TOut>>> adaptee, [NotNull] Func<TIn, TOut> select) : base(adaptee)
        {
            _select = @select;
        }

        protected override IUpdate<ListOperation<TOut>> HandleValue([NotNull] IUpdate<ListOperation<TIn>> value) =>
            new SelectConstantListUpdate<TIn,TOut>(value, _select);
    }
}
