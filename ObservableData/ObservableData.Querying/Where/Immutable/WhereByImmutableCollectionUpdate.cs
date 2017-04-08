using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Core;
using ObservableData.Querying.Utils.Adapters;

namespace ObservableData.Querying.Where.Immutable
{
    public sealed class WhereByImmutableCollectionUpdate<T> : SetUpdateAdapter<T, T>
    {
        [NotNull] private readonly Func<T, bool> _func;

        public WhereByImmutableCollectionUpdate([NotNull] IUpdate<SetOperation<T>> adaptee, [NotNull] Func<T, bool> func) : base(adaptee)
        {
            _func = func;
        }

        protected override IEnumerable<SetOperation<T>> Enumerate(IEnumerable<SetOperation<T>> adaptee)
        {
            foreach (var update in adaptee)
            {
                if (update.Type == SetOperationType.Clear)
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
