using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Core;
using ObservableData.Querying.Utils;
using ObservableData.Structures.Updates;

namespace ObservableData.Structures
{
    public class ObservableReadOnlyListAdapter<T> : IObservableData<T>
    {
        [NotNull] private readonly IObservableReadOnlyList<T> _adaptee;

        public ObservableReadOnlyListAdapter([NotNull] IObservableReadOnlyList<T> adaptee)
        {
            _adaptee = adaptee;
        }

        public void IgnoreEfficiency()
        {
        }

        public IDisposable Subscribe(IObserver<IUpdate<SetOperation<T>>> observer)
        {
            return _adaptee.Updates
                .Select(ListOperationExtensions.AsSetUpdate)
                .StartWith(new AddMutableSequenceToCollection<T>(_adaptee))
                .Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<T>>> observer)
        {
            return _adaptee.Updates
                .StartWith(new AddMutableSequenceToList<T>(_adaptee, 0))
                .Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<IUpdate<SetOperation<T>>> observer, out IReadOnlyCollection<T> mutableState)
        {
            mutableState = _adaptee;
            return _adaptee.Updates
                .Select(ListOperationExtensions.AsSetUpdate)
                .Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<T>>> observer, out IReadOnlyList<T> mutableState)
        {
            mutableState = _adaptee;
            return _adaptee.Updates.Subscribe(observer);
        }
    }
}
