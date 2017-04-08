using System;
using System.Reactive;
using System.Reactive.Linq;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils
{
    public static class ObservableExtensions
    {
        [NotNull]
        public static IObservable<T> AsWeakObservable<T>(this IObservable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Observable.Create<T>(o => source.WeakSubscribe(o));
        }

        [NotNull]
        public static IDisposable WeakSubscribe<T>([NotNull] this IObservable<T> source, Action<T> onNext)
        {
            return source.WeakSubscribe(Observer.Create(onNext));
        }

        [NotNull]
        public static IDisposable WeakSubscribe<T>([NotNull] this IObservable<T> source, IObserver<T> o)
        {
            var observer = new WeakObserver<T>(o);
            observer.SubscribeTo(source);

            return new WeakSubscription<T>(observer, o);
        }

        public static void KeepAlive<T>([NotNull] this IObservable<T> source, object keepAliveObject)
        {
            source.Subscribe(_ => GC.KeepAlive(keepAliveObject));
        }

        private sealed class WeakSubscription<T> : IDisposable
        {
            private WeakObserver<T> _weakObserver;
            private object _realObserver;

            public WeakSubscription(WeakObserver<T> weakObserver, object realObserver)
            {
                _weakObserver = weakObserver;
                _realObserver = realObserver;
            }

            public void Dispose()
            {
                GC.KeepAlive(_realObserver);
                _realObserver = null;
                _weakObserver?.Dispose();
                _weakObserver = null;
            }
        }

        private sealed class WeakObserver<T> : IObserver<T>, IDisposable
        {
            [CanBeNull]
            private IDisposable _strongSubscription;

            [NotNull]
            private readonly WeakReference<IObserver<T>> _observer;

            public WeakObserver(IObserver<T> observer)
            {
                _observer = new WeakReference<IObserver<T>>(observer);
            }

            public void OnNext(T value)
            {
                _observer.TryGetTarget(out var o);
                if (o != null)
                {
                    o.OnNext(value);
                }
                else
                {
                    this.Dispose();
                }
            }

            public void OnError(Exception exception)
            {
                _observer.TryGetTarget(out var o);
                o?.OnError(exception);
                this.Dispose();
            }

            public void OnCompleted()
            {
                _observer.TryGetTarget(out var o);
                o?.OnCompleted();
                this.Dispose();
            }

            public void SubscribeTo([NotNull] IObservable<T> source)
            {
                _strongSubscription = source.Subscribe(this);
            }

            public void Dispose()
            {
                _strongSubscription?.Dispose();
                _strongSubscription = null;
            }
        }
    }
}