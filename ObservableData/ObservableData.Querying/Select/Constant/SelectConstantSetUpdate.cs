using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters;

namespace ObservableData.Querying.Select.Constant
{
    public sealed class SelectConstantCollectionUpdate<TIn, TOut> : CollectionUpdateAdapter<TIn, TOut>
    {
        [NotNull] private readonly Func<TIn, TOut> _func;

        public SelectConstantCollectionUpdate([NotNull] IUpdate<CollectionOperation<TIn>> adaptee, [NotNull] Func<TIn, TOut> func) : base(adaptee)
        {
            _func = func;
        }

        protected override IEnumerable<CollectionOperation<TOut>> Enumerate(IEnumerable<CollectionOperation<TIn>> adaptee)
        {
            foreach (var update in adaptee)
            {
                switch (update.Type)
                {
                    case CollectionOperationType.Add:
                        yield return CollectionOperation<TOut>.OnAdd(_func(update.Item));
                        break;

                    case CollectionOperationType.Remove:
                        yield return CollectionOperation<TOut>.OnRemove(_func(update.Item));
                        break;

                    case CollectionOperationType.Clear:
                        yield return CollectionOperation<TOut>.OnClear();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}