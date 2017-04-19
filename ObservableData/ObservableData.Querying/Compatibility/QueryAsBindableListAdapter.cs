using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ObservableData.Querying.Compatibility
{
    public sealed class QueryAsBindableListAdapter<T> : 
        INotifyCollectionChanged,
        INotifyPropertyChanged, 
        IReadOnlyList<T>,
        IDisposable,
        IObserver<IUpdate<ListOperation<T>>>
    {
        [NotNull] private readonly IReadOnlyList<T> _state;
        [NotNull] private readonly IDisposable _subscription;

        public QueryAsBindableListAdapter([NotNull] IQuery<T> query)
        {
            _subscription = query.Subscribe(this, out _state);
        }

        public IEnumerator<T> GetEnumerator() => _state.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _state.Count;

        public T this[int index] => _state[index];

        public void OnCompleted() => this.Dispose();

        public void OnError(Exception error) => this.Dispose();

        public void OnNext([NotNull] IUpdate<ListOperation<T>> value)
        {
            foreach (var update in value.Operations())
            {
                if (update.Type == ListOperationType.Clear)
                {
                    OnClear();
                    break;
                }

                switch (update.Type)
                {
                    case ListOperationType.Add:
                        OnAdd(update);
                        break;

                    case ListOperationType.Remove:
                        OnRemove(update);
                        break;

                    case ListOperationType.Move:
                        OnMove(update);
                        break;

                    case ListOperationType.Replace:
                        OnReplace(update);
                        break;

                    case ListOperationType.Clear:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            OnPropertyChanged(nameof(Count));
        }

        private void OnMove(ListOperation<T> update)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, update.Item, update.Index, update.OriginalIndex);
            this.CollectionChanged?.Invoke(this, args);
        }

        private void OnRemove(ListOperation<T> update)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, update.Item, update.Index);
            this.CollectionChanged?.Invoke(this, args);
        }


        private void OnAdd(ListOperation<T> update)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, update.Item, update.Index);
            this.CollectionChanged?.Invoke(this, args);
        }

        private void OnReplace(ListOperation<T> update)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, update.Item, update.ChangedItem, update.Index);
            this.CollectionChanged?.Invoke(this, args);
        }

        private void OnClear()
        {
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
