using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters;
using ObservableData.Structures;

namespace ObservableData.Querying.Select.Constant
{
    public sealed class SelectConstantListUpdate<TIn, TOut> : ListUpdateAdapter<TIn, TOut>
    {
        [NotNull] private readonly Func<TIn, TOut> _func;

        public SelectConstantListUpdate([NotNull] IUpdate<ListOperation<TIn>> adaptee, [NotNull] Func<TIn, TOut> func) : base(adaptee)
        {
            _func = func;
        }

        protected override IEnumerable<ListOperation<TOut>> Enumerate(IEnumerable<ListOperation<TIn>> adaptee)
        {
            foreach (var update in adaptee)
            {
                switch (update.Type)
                {
                    case ListOperationType.Add:
                        yield return ListOperation<TOut>.OnAdd(_func(update.Item), update.Index);
                        break;

                    case ListOperationType.Remove:
                        yield return ListOperation<TOut>.OnRemove(_func(update.Item), update.Index);
                        break;

                    case ListOperationType.Move:
                        yield return ListOperation<TOut>.OnMove(_func(update.Item), update.Index, update.OriginalIndex);
                        break;

                    case ListOperationType.Replace:
                        yield return ListOperation<TOut>.OnReplace(_func(update.Item), _func(update.ChangedItem),
                            update.Index);
                        break;

                    case ListOperationType.Clear:
                        yield return ListOperation<TOut>.OnClear();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}