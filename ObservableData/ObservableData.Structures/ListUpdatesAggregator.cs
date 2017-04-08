using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using ObservableData.Querying.Core;
using ObservableData.Structures.Updates;

namespace ObservableData.Structures
{
    public class ListUpdatesAggregator<T> : IObservable<IUpdate<ListOperation<T>>>
    {
        [NotNull] private readonly Subject<IUpdate<ListOperation<T>>> _subject = new Subject<IUpdate<ListOperation<T>>>();

        [CanBeNull] private BatchUpdate<ListOperation<T>> _batch;


        [NotNull]
        public IDisposable StartBatchUpdate()
        {
            if (_batch != null) throw new NotSupportedException("recusive batch updates not supported");

            _batch = new BatchUpdate<ListOperation<T>>(this.StopBatchUpdate);
            return _batch;
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<T>>> observer)
        {
            return _subject.Subscribe(observer);
        }

        private void StopBatchUpdate(BatchUpdate<ListOperation<T>> batch)
        {
            if (batch != _batch) return;
            _batch = null;

            _subject.OnNext(batch);
        }

        public void OnAdd([NotNull] IEnumerable<T> items, int index)
        {
            if (ShouldNotify)
            {
                var update = new AddMutableSequenceToList<T>(items, index);
                OnNext(update);
            }
        }

        public void OnAdd(T item, int index)
        {
            if (ShouldNotify)
            {
                var update = new AddItemToList<T>(item, index);
                OnNext(update);
            }
        }

        public void OnReset(IEnumerable<T> items)
        {
            _batch?.Clear();
            if (ShouldNotify)
            {
                IQuickLinkableUpdate<ListOperation<T>> update;
                if (items == null)
                {
                    update = ClearList<T>.Instance; 
                }
                else
                {
                    update = new ResetWithMutableSequenceToList<T>(items);
                }
                OnNext(update);
            }
        }

        public void OnMove(T item, int from, int to)
        {
            if (ShouldNotify)
            {
                var update = new MoveAtList<T>(item, from, to);
                OnNext(update);
            }
        }

        public void OnRemove(T item, int index)
        {
            if (ShouldNotify)
            {
                var update = new RemoveAtList<T>(item, index);
                OnNext(update);
            }
        }

        public void OnReplace(T value, T changedItem, int index)
        {
            if (ShouldNotify)
            {
                var update = new ReplaceAtList<T>(value, changedItem, index);
                OnNext(update);
            }
        }

        public void OnClear()
        {
            _batch?.Clear();
            if (ShouldNotify)
            {
                OnNext(ClearList<T>.Instance);
            }
        }

        private bool ShouldNotify => _batch != null || _subject.HasObservers;

        private void OnNext(IQuickLinkableUpdate<ListOperation<T>> update)
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
