using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Core;

namespace ObservableData.Querying.Select.Immutable
{
    public sealed class SelectImmutableData<TIn, TOut> : IObservableData<TOut>
    {
        [NotNull] private readonly IObservableData<TIn> _previous;
        [NotNull] private readonly Func<TIn, TOut> _func;

        public SelectImmutableData([NotNull] Func<TIn, TOut> func, [NotNull] IObservableData<TIn> previous)
        {
            _previous = previous;
            _func = func;
        }

        public void IgnoreEfficiency() { }

        public IDisposable Subscribe(IObserver<IUpdate<SetOperation<TOut>>> observer)
        {
            var select = new SelectImmutableObserver<TIn, TOut>(_func);
            select.SetAdaptee(observer);
            return _previous.Subscribe(select);
        }

        public IDisposable Subscribe(IObserver<IUpdate<SetOperation<TOut>>> observer, out IReadOnlyCollection<TOut> mutableState)
        {
            var select = new SelectImmutableObserver<TIn, TOut>(_func);
            var subscription = _previous.Subscribe(select);
            mutableState = select;
            select.SetAdaptee(observer);

            return subscription;
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<TOut>>> observer)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<TOut>>> observer, out IReadOnlyList<TOut> mutableState)
        {
            throw new NotImplementedException();
        }
    }
}
