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

            return new KeedAliveDisposableAdapter(observer, o);
        }

        public static void KeepAlive<T>([NotNull] this IObservable<T> source, object keepAliveObject)
        {
            source.Subscribe(_ => GC.KeepAlive(keepAliveObject));
        }

        private sealed class WeakObserver<T> : IObserver<T>, IDisposable
        {
            [CanBeNull]
            private IDisposable _strongSubscription;

            [NotNull]
            private readonly WeakReference<IObserver<T>> _observer;

            public WeakObserver(IObserver<T> underlying)
            {
                _observer = new WeakReference<IObserver<T>>(underlying);
            }

            public void OnNext(T value)
            {
                IObserver<T> o;
                _observer.TryGetTarget(out o);
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
                IObserver<T> o;
                _observer.TryGetTarget(out o);
                o?.OnError(exception);
                this.Dispose();
            }

            public void OnCompleted()
            {
                IObserver<T> o;
                _observer.TryGetTarget(out o);
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

        public sealed class KeedAliveDisposableAdapter : IDisposable
        {
            private IDisposable _disposable;
            private object _keepAliveObject;

            public KeedAliveDisposableAdapter(IDisposable disposable, object keepAliveObject)
            {
                _disposable = disposable;
                _keepAliveObject = keepAliveObject;
            }

            public void Dispose()
            {
                GC.KeepAlive(_keepAliveObject);
                _keepAliveObject = null;
                _disposable?.Dispose();
                _disposable = null;
            }
        }
    }

    public static class WeakReferenceExtensions
    {
        [CanBeNull]
        public static T TryGetTarget<T>([NotNull] this WeakReference<T> reference) where T : class
        {
            T o;
            reference.TryGetTarget(out o);
            return o;
        }
    }
}