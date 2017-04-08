using System;
using JetBrains.Annotations;
using ObservableData.Querying.Core;
using System.Reactive.Linq;

namespace ObservableData.Querying.Select.Constant
{



    public class SelectConstantCollectionObserver<TIn, TOut> : IObserver<ISetUpdate<TIn>>
    {
        [NotNull] private readonly IObserver<ISetUpdate<TOut>> _adaptee;
        [NotNull] private readonly Func<TIn, TOut> _select;

        public SelectConstantCollectionObserver(
            [NotNull] IObserver<ISetUpdate<TOut>> adaptee,
            [NotNull] Func<TIn, TOut> select)
        {
            _adaptee = adaptee;
            _select = select;
        }

        public void OnCompleted() => _adaptee.OnCompleted();

        public void OnError(Exception error) => _adaptee.OnError(error);

        public void OnNext([NotNull] ISetUpdate<TIn> value)
        {
            var update = new SelectConstantSetUpdate<TIn, TOut>(value, _select);
            _adaptee.OnNext(update);
        }
    }
}
