using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying;

namespace ObservableData.Structures.Lists.Updates
{
    public class ListBatchUpdate<T> : IDisposable, IListUpdate<T>
    {
        [NotNull] private readonly Action<ListBatchUpdate<T>> _onDispose;
        private ListBaseOperation<T> _first;
        private ListBaseOperation<T> _last;

        public ListBatchUpdate([NotNull] Action<ListBatchUpdate<T>> onDispose)
        {
            _onDispose = onDispose;
        }

        public void Add(ListBaseOperation<T> update)
        {
            if (_last == null)
            {
                _first = update;
                _last = update;
            }
            else
            {
                _last.Next = update;
                _last = update;
            }
        }

        public void Clear()
        {
            _last = null;
            _first = null;
        }

        public void Dispose()
        {
            _onDispose(this);
        }

        public void Lock()
        {
            var next = _first;
            while (next != null)
            {
                next.Lock();
                next = next.Next;
            }
        }

        IEnumerable<ICollectionOperation<T>> IUpdate<ICollectionOperation<T>>.Operations()
        {
            var next = _first;
            while (next != null)
            {
                var operation = next.TryGetCollectionOperation();
                if (operation != null)
                {
                    yield return operation;
                }
                next = next.Next;
            }
        }

        IEnumerable<IListOperation<T>> IUpdate<IListOperation<T>>.Operations()
        {
            var next = _first;
            while (next != null)
            {
                yield return next;
                next = next.Next;
            }
        }
    }
}
