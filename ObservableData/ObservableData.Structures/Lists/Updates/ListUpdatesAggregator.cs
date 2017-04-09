using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using ObservableData.Querying;

namespace ObservableData.Structures.Lists.Updates
{
    public class ListUpdatesAggregator<T> : IObservable<IUpdate<IListOperation<T>>>, IObservable<IUpdate<ICollectionOperation<T>>>
    {
        [NotNull] private readonly Subject<IListUpdate<T>> _subject = new Subject<IListUpdate<T>>();

        [CanBeNull] private ListBatchUpdate<T> _batch;

        private bool ShouldNotify => _batch != null || _subject.HasObservers;

        [NotNull]
        public IDisposable StartBatchUpdate()
        {
            if (_batch != null) throw new NotImplementedException("recusive batch updates are not implemented");

            _batch = new ListBatchUpdate<T>(this.StopBatchUpdate);
            return _batch;
        }

        public IDisposable Subscribe(IObserver<IUpdate<IListOperation<T>>> observer)
        {
            return _subject.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<IUpdate<ICollectionOperation<T>>> observer)
        {
            return _subject.Subscribe(observer);
        }

        private void StopBatchUpdate(ListBatchUpdate<T> batch)
        {
            if (batch != _batch) return;
            _batch = null;

            _subject.OnNext(batch);
        }

        public void OnAdd([NotNull] IEnumerable<T> items, int index)
        {
            if (ShouldNotify)
            {
                var update = new ListInsertBatchOperation<T>(items, index);
                OnNext(update);
            }
        }

        public void OnAdd(T item, int index)
        {
            if (ShouldNotify)
            {
                var update = new ListInsertItemOperation<T>(item, index);
                OnNext(update);
            }
        }

        public void OnReset(IEnumerable<T> items)
        {
            _batch?.Clear();
            if (ShouldNotify)
            {
                var update = new ListResetOperation<T>(items);
                OnNext(update);
            }
        }

        public void OnMove(T item, int from, int to)
        {
            if (ShouldNotify)
            {
                var update = new ListMoveOperation<T>(item, from, to);
                OnNext(update);
            }
        }

        public void OnRemove(T item, int index)
        {
            if (ShouldNotify)
            {
                var update = new ListRemoveOperation<T>(item, index);
                OnNext(update);
            }
        }

        public void OnReplace(T value, T changedItem, int index)
        {
            if (ShouldNotify)
            {
                var update = new ListReplaceOperation<T>(index, value, changedItem);
                OnNext(update);
            }
        }

        private void OnNext(ListBaseOperation<T> update)
        {
            if (_batch != null)
            {
                _batch.Add(update);
            }
            else
            {
                _subject.OnNext(update);
            }
        }

    }
}
