using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters;
using ObservableData.Structures;

namespace ObservableData.Querying.Where.Immutable
{
    public sealed class WhereByImmutableCollectionUpdate<T> : CollectionUpdateAdapter<T, T>
    {
        [NotNull] private readonly Func<T, bool> _func;

        public WhereByImmutableCollectionUpdate([NotNull] IUpdate<CollectionOperation<T>> adaptee, [NotNull] Func<T, bool> func) : base(adaptee)
        {
            _func = func;
        }

        protected override IEnumerable<CollectionOperation<T>> Enumerate(IEnumerable<CollectionOperation<T>> adaptee)
        {
            foreach (var update in adaptee)
            {
                if (update.Type == CollectionOperationType.Clear)
                {
                    yield return update;
                }
                else if (_func.Invoke(update.Item))
                {
                    yield return update;
                }
            }
        }
    }
}
