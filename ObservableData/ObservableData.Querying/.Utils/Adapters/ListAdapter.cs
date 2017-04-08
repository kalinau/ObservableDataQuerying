using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Core;

namespace ObservableData.Querying.Utils.Adapters
{
    public abstract class ListAdapter<TIn, TOut> : IReadOnlyList<TOut>
    {
        [NotNull] private readonly IReadOnlyList<TIn> _source;

        protected ListAdapter([NotNull] IReadOnlyList<TIn> source)
        {
            _source = source;
        }

        public int Count => _source.Count;

        public IEnumerator<TOut> GetEnumerator() => Enumerate(_source).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public TOut this[int index] => Select(_source[index]);

        [NotNull]
        protected abstract IEnumerable<TOut> Enumerate(IEnumerable<TIn> source);

        protected abstract TOut Select(TIn item);
    }



    public abstract class UpdateAdapter<TIn, TOut> : IUpdate<TOut>
    {
        [NotNull] private readonly IUpdate<TIn> _adaptee;

        protected UpdateAdapter([NotNull] IUpdate<TIn> adaptee)
        {
            _adaptee = adaptee;
        }

        public IEnumerable<TOut> Operations() => Enumerate(_adaptee.Operations());

        [NotNull]
        protected abstract IEnumerable<TOut> Enumerate([NotNull] IEnumerable<TIn> adaptee);

        public void Lock() => _adaptee.Lock();
    }

    public abstract class SetUpdateAdapter<TIn, TOut> : UpdateAdapter<SetOperation<TIn>, SetOperation<TOut>>
    {
        protected SetUpdateAdapter([NotNull] IUpdate<SetOperation<TIn>> adaptee) : base(adaptee)
        {
        }
    }

    public abstract class ListUpdateAdapter<TIn, TOut> : UpdateAdapter<ListOperation<TIn>, ListOperation<TOut>>
    {
        protected ListUpdateAdapter([NotNull] IUpdate<ListOperation<TIn>> adaptee) : base(adaptee)
        {
        }
    }

    public abstract class ObserverAdapter<TIn, TOut> : IObserver<TIn>
    {
        [NotNull] private readonly IObserver<TOut> _adaptee;

        protected ObserverAdapter([NotNull] IObserver<TOut> adaptee)
        {
            _adaptee = adaptee;
        }

        public void OnCompleted() => _adaptee.OnCompleted();

        public void OnError(Exception error) => _adaptee.OnError(error);

        public void OnNext([NotNull] TIn value) => _adaptee.OnNext(this.HandleValue(value));

        protected abstract TOut HandleValue(TIn value);
    }

    public class AnonymousObserverAdapter<TIn, TOut> : ObserverAdapter<TIn, TOut>
    {
        [NotNull] private readonly Func<TIn, TOut> _handler;

        public AnonymousObserverAdapter(
            [NotNull] IObserver<TOut> adaptee,
            [NotNull] Func<TIn, TOut> handler) : base(adaptee)
        {
            _handler = handler;
        }

        protected override TOut HandleValue(TIn value) => _handler(value);
    }

    public abstract class SetObserverAdapter<TIn, TOut> : ObserverAdapter<IUpdate<SetOperation<TIn>>, IUpdate<SetOperation<TOut>>>
    {
        protected SetObserverAdapter([NotNull] IObserver<IUpdate<SetOperation<TOut>>> adaptee) : base(adaptee)
        {
        }
    }

    public abstract class ListObserverAdapter<TIn, TOut> :
        ObserverAdapter<IUpdate<ListOperation<TIn>>, IUpdate<ListOperation<TOut>>>
    {
        protected ListObserverAdapter([NotNull] IObserver<IUpdate<ListOperation<TOut>>> adaptee) : base(adaptee)
        {
        }
    }

    public sealed class SetAnonymousObserverAdapter<TIn, TOut> : AnonymousObserverAdapter<IUpdate<SetOperation<TIn>>, IUpdate<SetOperation<TOut>>>
    {
        public SetAnonymousObserverAdapter(
            [NotNull] IObserver<IUpdate<SetOperation<TOut>>> adaptee,
            [NotNull] Func<IUpdate<SetOperation<TIn>>, IUpdate<SetOperation<TOut>>> handler) 
            : base(adaptee, handler)
        {
        }
    }

    public sealed class ListAnonymousObserverAdapter<TIn, TOut> :
        AnonymousObserverAdapter<IUpdate<ListOperation<TIn>>, IUpdate<ListOperation<TOut>>>
    {
        public ListAnonymousObserverAdapter([NotNull] IObserver<IUpdate<ListOperation<TOut>>> adaptee, [NotNull] Func<IUpdate<ListOperation<TIn>>, IUpdate<ListOperation<TOut>>> handler) : base(adaptee, handler)
        {
        }
    }

    public static class DataObserver
    {
        [NotNull]
        public static IObserver<IUpdate<SetOperation<TIn>>> Create<TIn, TOut>(
            [NotNull] IObserver<IUpdate<SetOperation<TOut>>> adaptee,
            [NotNull] Func<IUpdate<SetOperation<TIn>>, IUpdate<SetOperation<TOut>>> handler)
        {
            return new SetAnonymousObserverAdapter<TIn, TOut>(adaptee, handler);
        }

        [NotNull]
        public static IObserver<IUpdate<ListOperation<TIn>>> Create<TIn, TOut>(
            [NotNull] IObserver<IUpdate<ListOperation<TOut>>> adaptee,
            [NotNull] Func<IUpdate<ListOperation<TIn>>, IUpdate<ListOperation<TOut>>> handler)
        {
            return new ListAnonymousObserverAdapter<TIn, TOut>(adaptee, handler);
        }
    }
}
