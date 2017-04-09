using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Structures.Lists.Updates
{
    public class ListResetOperation<T> : ListInsertBatchOperation<T>, IListResetOperation<T>, ICollectionResetOperation<T>
    {
        public ListResetOperation([CanBeNull] IEnumerable<T> items) : base(items, 0)
        {
        }

        public override void Match(Action<IListInsertOperation<T>> onInsert, Action<IListRemoveOperation<T>> onRemove, Action<IListReplaceOperation<T>> onReplace, Action<IListMoveOperation<T>> onMove, Action<IListResetOperation<T>> onReset)
        {
            onReset?.Invoke(this);
        }

        public override TResult Match<TResult>(Func<IListInsertOperation<T>, TResult> onInsert, Func<IListRemoveOperation<T>, TResult> onRemove, Func<IListReplaceOperation<T>, TResult> onReplace, Func<IListMoveOperation<T>, TResult> onMove, Func<IListResetOperation<T>, TResult> onReset)
        {
            return onReset.Invoke(this);
        }

        public override TResult Match<TResult>(Func<ICollectionInsertOperation<T>, TResult> onInsert, Func<ICollectionRemoveOperation<T>, TResult> onRemove, Func<ICollectionReplaceOperation<T>, TResult> onReplace, Func<ICollectionResetOperation<T>, TResult> onReset)
        {
            return onReset.Invoke(this);
        }

        public override void Match(Action<ICollectionInsertOperation<T>> onInsert, Action<ICollectionRemoveOperation<T>> onRemove, Action<ICollectionReplaceOperation<T>> onReplace, Action<ICollectionResetOperation<T>> onReset)
        {
            onReset?.Invoke(this);
        }
    }
}