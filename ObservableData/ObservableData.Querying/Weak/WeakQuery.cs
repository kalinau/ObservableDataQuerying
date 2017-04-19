using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Weak
{
    public sealed class WeakSubscribableQuery<T> : IQuery<T>
    {
        [NotNull] private readonly IQuery<T> _previous;

        public WeakSubscribableQuery([NotNull] IQuery<T> previous)
        {
            _previous = previous;
        }

        public void IgnoreEfficiency()
        {
        }

        public IDisposable Subscribe(IObserver<IUpdate<CollectionOperation<T>>> o)
        {
            var observer = new WeakObserver<IUpdate<CollectionOperation<T>>>(o);
            var strongSubscription = _previous.Subscribe(observer);
            observer.SaveSubscription(strongSubscription);

            return new KeedAliveDisposableAdapter(observer, o);

        }

        public IDisposable Subscribe(
            IObserver<IUpdate<CollectionOperation<T>>> o, 
            out IReadOnlyCollection<T> mutableState)
        {
            var observer = new WeakObserver<IUpdate<CollectionOperation<T>>>(o);
            var strongSubscription = _previous.Subscribe(observer, out mutableState);
            observer.SaveSubscription(strongSubscription);

            return new KeedAliveDisposableAdapter(observer, o);
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<T>>> o)
        {
            var observer = new WeakObserver<IUpdate<ListOperation<T>>>(o);
            var strongSubscription = _previous.Subscribe(observer);
            observer.SaveSubscription(strongSubscription);

            return new KeedAliveDisposableAdapter(observer, o);
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<T>>> o, out IReadOnlyList<T> mutableState)
        {
            var observer = new WeakObserver<IUpdate<ListOperation<T>>>(o);
            var strongSubscription = _previous.Subscribe(observer, out mutableState);
            observer.SaveSubscription(strongSubscription);

            return new KeedAliveDisposableAdapter(observer, o);
        }
    }
}
