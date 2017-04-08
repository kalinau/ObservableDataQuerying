using System;
using JetBrains.Annotations;
using ObservableData.Querying.Core;

namespace ObservableData.Querying.Select.Constant
{
    public sealed class SelectConstantListObserver<TIn, TOut> : IObserver<IListUpdate<TIn>>
    {
        [NotNull] private readonly IObserver<IListUpdate<TOut>> _adaptee;
        [NotNull] private readonly Func<TIn, TOut> _select;

        public SelectConstantListObserver(
            [NotNull] IObserver<IListUpdate<TOut>> adaptee,
            [NotNull] Func<TIn, TOut> select)
        {
            _adaptee = adaptee;
            _select = select;
        }

        public void OnCompleted() => _adaptee.OnCompleted();

        public void OnError(Exception error) => _adaptee.OnError(error);

        public void OnNext([NotNull] IListUpdate<TIn> update)
        {
            var update = new SelectConstantListUpdate<TIn,TOut>(mutation, _select);
            _adaptee.OnNext(update);
        }
    }
}
