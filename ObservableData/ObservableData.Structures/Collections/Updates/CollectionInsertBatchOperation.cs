using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Structures.Collections.Updates
{
    public class CollectionInsertBatchOperation<T> : CollectionBaseOperation<T>, ICollectionInsertOperation<T>
    {
        [NotNull] private readonly IEnumerable<T> _items;
        [CanBeNull] private IEnumerable<T> _locked;

        private readonly ThreadId _threadId;

        public CollectionInsertBatchOperation([NotNull] IEnumerable<T> items)
        {
            _items = items;
            _threadId = ThreadId.FromCurrent();
        }

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

        public override TResult Match<TResult>(Func<ICollectionInsertOperation<T>, TResult> onInsert, Func<ICollectionRemoveOperation<T>, TResult> onRemove, Func<ICollectionReplaceOperation<T>, TResult> onReplace, Func<ICollectionResetOperation<T>, TResult> onReset)
        {
            return onInsert.Invoke(this);
        }

        public override void Match(Action<ICollectionInsertOperation<T>> onInsert, Action<ICollectionRemoveOperation<T>> onRemove, Action<ICollectionReplaceOperation<T>> onReplace, Action<ICollectionResetOperation<T>> onReset)
        {
            onInsert?.Invoke(this);
        }
    }
}
