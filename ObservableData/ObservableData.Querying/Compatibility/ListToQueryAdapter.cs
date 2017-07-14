using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Structures;

namespace ObservableData.Querying.Compatibility
{
    public class ListToQueryAdapter<T> : IQuery<T>
    {
        [NotNull] private readonly IObservableReadOnlyList<T> _adaptee;

        public ListToQueryAdapter([NotNull] IObservableReadOnlyList<T> adaptee)
        {
            _adaptee = adaptee;
        }

        public void IgnoreEfficiency()
        {
        }

        public IDisposable Subscribe(IObserver<IUpdate<CollectionOperation<T>>> observer)
        {
            IObservableReadOnlyCollection<T> collection = _adaptee;
            return collection.AsObservable().Select(Adapt).Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<T>>> observer)
        {
            return _adaptee.AsObservable().Select(Adapt).Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<IUpdate<CollectionOperation<T>>> observer, out IReadOnlyCollection<T> mutableState)
        {
            IObservableReadOnlyCollection<T> collection = _adaptee;
            mutableState = collection;
            return collection.Updates.Select(Adapt).Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<T>>> observer, out IReadOnlyList<T> mutableState)
        {
            mutableState = _adaptee;
            return _adaptee.Updates.Select(Adapt).Subscribe(observer);
        }

        private static ListUpdateToQueryUpdates<T> Adapt([NotNull] IUpdate<IListOperation<T>> x)
        {
            return new ListUpdateToQueryUpdates<T>(x);
        }

        private static CollectionUpdateToQueryUpdates<T> Adapt([NotNull] IUpdate<ICollectionOperation<T>> x)
        {
            return new CollectionUpdateToQueryUpdates<T>(x);
        }
    }
}
