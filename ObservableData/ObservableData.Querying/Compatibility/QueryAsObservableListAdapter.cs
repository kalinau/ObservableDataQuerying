﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;
using ObservableData.Structures;
using ObservableData.Structures.Lists.Updates;

namespace ObservableData.Querying.Compatibility
{
    public class QueryAsObservableListAdapter<T> :
        IObservableReadOnlyList<T>,
        IDisposable,
        IObserver<IUpdate<ListOperation<T>>>
    {
        [NotNull] private readonly IReadOnlyList<T> _state;
        [NotNull] private readonly IDisposable _subscription;
        [NotNull] private readonly Subject<IListUpdate<T>> _subject = 
            new Subject<IListUpdate<T>>();

        public QueryAsObservableListAdapter([NotNull] IQuery<T> query)
        {
            _subject.KeepAlive(this);
            _subscription = query.Subscribe(this, out _state);
        }

        public IEnumerator<T> GetEnumerator() => _state.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public int Count => _state.Count;

        public T this[int index] => _state[index];


        IObservable<IUpdate<IListOperation<T>>> IObservableReadOnlyList<T>.Updates => _subject;

        IObservable<IUpdate<ICollectionOperation<T>>> IObservableReadOnlyCollection<T>.Updates => _subject;

        public void Dispose()
        {
            _subscription.Dispose();
        }

        public void OnCompleted() => this.Dispose();

        public void OnError(Exception error) => this.Dispose();

        public void OnNext(IUpdate<ListOperation<T>> value)
        {
            _subject.OnNext(null);
        }
    }
}