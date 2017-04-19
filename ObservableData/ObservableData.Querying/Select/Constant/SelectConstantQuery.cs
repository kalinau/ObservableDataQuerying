using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying.Select.Constant
{
    public sealed class SelectConstantQuery<TIn, TOut> : IQuery<TOut>
    {
        [NotNull] private readonly IQuery<TIn> _previous;
        [NotNull] private readonly Func<TIn, TOut> _func;

        public SelectConstantQuery([NotNull] IQuery<TIn> previous, [NotNull] Func<TIn, TOut> func)
        {
            _previous = previous;
            _func = func;
        }

        public void IgnoreEfficiency() { }

        public IDisposable Subscribe(IObserver<IUpdate<CollectionOperation<TOut>>> observer)
        {
            var adapter = new SelectConstantCollectionObserver<TIn, TOut>(observer, _func);
            return _previous.Subscribe(adapter);
        }

        public IDisposable Subscribe(IObserver<IUpdate<CollectionOperation<TOut>>> observer, out IReadOnlyCollection<TOut> mutableState)
        {
            var adapter = new SelectConstantCollectionObserver<TIn, TOut>(observer, _func);

            IReadOnlyCollection<TIn> previousState;
            var result = _previous.Subscribe(adapter, out previousState);
            mutableState = new SelectConstantCollection<TIn, TOut>(previousState, _func);
            return result;
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<TOut>>> observer)
        {
            var adapter = new SelectConstantListObserver<TIn, TOut>(observer, _func);
            return _previous.Subscribe(adapter);
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<TOut>>> observer, out IReadOnlyList<TOut> mutableState)
        {
            var adapter = new SelectConstantListObserver<TIn, TOut>(observer, this._func);

            IReadOnlyList<TIn> previousState;
            var result = _previous.Subscribe(adapter, out previousState);
            mutableState = new SelectConstantList<TIn, TOut>(previousState, _func);
            return result;
        }
    }
}
