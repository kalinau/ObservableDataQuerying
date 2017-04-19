using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying;

namespace ObservableData.Structures.Utils
{
    public sealed class ListUpdateToQueryUpdates<T> : IUpdate<ListOperation<T>>
    {
        [NotNull] private readonly IUpdate<IListOperation<T>> _adaptee;

        public ListUpdateToQueryUpdates([NotNull] IUpdate<IListOperation<T>> adaptee)
        {
            _adaptee = adaptee;
        }

        public void Lock() => _adaptee.Lock();

        public IEnumerable<ListOperation<T>> Operations()
        {
            foreach (var operation in _adaptee.Operations())
            {
                var sum = operation.Match(
                    OnInsert,
                    OnRemove,
                    OnReplace,
                    OnMove,
                    OnReset
                );
                if (sum.Items != null)
                {
                    foreach (var item in sum.Items)
                    {
                        yield return item;
                    }
                }
                else
                {
                    yield return sum.Item;
                }
            }
        }

        private static ItemOrEnumerable<ListOperation<T>> OnInsert([NotNull] IListInsertOperation<T> insert) =>
            new ItemOrEnumerable<ListOperation<T>>(Enumerate(insert));

        private static ItemOrEnumerable<ListOperation<T>> OnRemove([NotNull] IListRemoveOperation<T> remove) =>
            new ItemOrEnumerable<ListOperation<T>>(ListOperation<T>.OnRemove(remove.Item, remove.Index));

        private static ItemOrEnumerable<ListOperation<T>> OnReplace([NotNull] IListReplaceOperation<T> replace) =>
            new ItemOrEnumerable<ListOperation<T>>(ListOperation<T>.OnReplace(replace.Item, replace.ReplacedItem, replace.Index));

        private static ItemOrEnumerable<ListOperation<T>> OnMove([NotNull] IListMoveOperation<T> move) =>
            new ItemOrEnumerable<ListOperation<T>>(ListOperation<T>.OnMove(move.Item, move.To, move.From));

        private static ItemOrEnumerable<ListOperation<T>> OnReset([NotNull] IListResetOperation<T> reset) => 
            new ItemOrEnumerable<ListOperation<T>>(Enumerate(reset));

        private static IEnumerable<ListOperation<T>> Enumerate([NotNull] IListInsertOperation<T> insert)
        {
            var index = insert.Index;
            foreach (var item in insert.Items)
            {
                yield return ListOperation<T>.OnAdd(item, index++);
            }
        }

        private static IEnumerable<ListOperation<T>> Enumerate([NotNull] IListResetOperation<T> reset)
        {
            yield return ListOperation<T>.OnClear();
            var index = 0;
            if (reset.Items != null)
            {
                foreach (var item in reset.Items)
                {
                    yield return ListOperation<T>.OnAdd(item, index++);
                }
            }
        }
    }
}
