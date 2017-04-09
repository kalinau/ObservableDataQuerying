using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Structures.Lists.Updates
{
    public class ListInsertBatchOperation<T> : ListBaseOperation<T>, IListInsertOperation<T>, ICollectionInsertOperation<T>
    {
        private readonly int _index;
        [NotNull] private readonly IEnumerable<T> _items;
        [CanBeNull] private IEnumerable<T> _locked;

        private readonly ThreadId _threadId;

        public ListInsertBatchOperation([NotNull] IEnumerable<T> items, int index)
        {
            _items = items;
            _index = index;
            _threadId = ThreadId.FromCurrent();
        }

        public int Index => _index;

        public IEnumerable<T> Items
        {
            get
            {
                if (_locked != null)
                {
                    return _locked;
                }
                _threadId.CheckIsCurrent();
                return _items;
            }
        }

        public override void Lock()
        {
            if (_locked == null)
            {
                _locked = _items.ToList();
            }
        }

        public override TResult Match<TResult>(Func<IListInsertOperation<T>, TResult> onInsert, Func<IListRemoveOperation<T>, TResult> onRemove, Func<IListReplaceOperation<T>, TResult> onReplace, Func<IListMoveOperation<T>, TResult> onMove, Func<IListResetOperation<T>, TResult> onReset)
        {
            return onInsert.Invoke(this);
        }

        public override void Match(Action<IListInsertOperation<T>> onInsert, Action<IListRemoveOperation<T>> onRemove, Action<IListReplaceOperation<T>> onReplace, Action<IListMoveOperation<T>> onMove, Action<IListResetOperation<T>> onReset)
        {
            onInsert?.Invoke(this);
        }

        public virtual TResult Match<TResult>(Func<ICollectionInsertOperation<T>, TResult> onInsert, Func<ICollectionRemoveOperation<T>, TResult> onRemove, Func<ICollectionReplaceOperation<T>, TResult> onReplace, Func<ICollectionResetOperation<T>, TResult> onReset)
        {
            return onInsert.Invoke(this);
        }

        public virtual void Match(Action<ICollectionInsertOperation<T>> onInsert, Action<ICollectionRemoveOperation<T>> onRemove, Action<ICollectionReplaceOperation<T>> onReplace, Action<ICollectionResetOperation<T>> onReset)
        {
            onInsert?.Invoke(this);
        }

        public override ICollectionOperation<T> TryGetCollectionOperation() => this;
    }
}