using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

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

        public IEnumerator<TOut> GetEnumerator() => this.Enumerate(_source).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public TOut this[int index] => this.Select(_source[index]);

        [NotNull]
        protected abstract IEnumerable<TOut> Enumerate([NotNull] IEnumerable<TIn> source);

        protected abstract TOut Select(TIn item);
    }



    public abstract class UpdateAdapter<TIn, TOut> : IUpdate<TOut>
    {
        [NotNull] private readonly IUpdate<TIn> _adaptee;

        protected UpdateAdapter([NotNull] IUpdate<TIn> adaptee)
        {
            _adaptee = adaptee;
        }

        public IEnumerable<TOut> Operations() => this.Enumerate(_adaptee.Operations());

        [NotNull]
        protected abstract IEnumerable<TOut> Enumerate([NotNull] IEnumerable<TIn> adaptee);

        public void Lock() => _adaptee.Lock();
    }

    public abstract class CollectionUpdateAdapter<TIn, TOut> : UpdateAdapter<CollectionOperation<TIn>, CollectionOperation<TOut>>
    {
        protected CollectionUpdateAdapter([NotNull] IUpdate<CollectionOperation<TIn>> adaptee) : base(adaptee)
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

    public abstract class CollectionObserverAdapter<TIn, TOut> : ObserverAdapter<IUpdate<CollectionOperation<TIn>>, IUpdate<CollectionOperation<TOut>>>
    {
        protected CollectionObserverAdapter([NotNull] IObserver<IUpdate<CollectionOperation<TOut>>> adaptee) : base(adaptee)
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
}
