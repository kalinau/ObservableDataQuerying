using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Core;
using ObservableData.Querying.Utils.Adapters;

namespace ObservableData.Querying.Select.Constant
{
    public sealed class SelectConstantSetUpdate<TIn, TOut> : SetUpdateAdapter<TIn, TOut>
    {
        [NotNull] private readonly Func<TIn, TOut> _func;

        public SelectConstantSetUpdate([NotNull] IUpdate<SetOperation<TIn>> adaptee, [NotNull] Func<TIn, TOut> func) : base(adaptee)
        {
            _func = func;
        }

        protected override IEnumerable<SetOperation<TOut>> Enumerate(IEnumerable<SetOperation<TIn>> adaptee)
        {
            foreach (var update in adaptee)
            {
                switch (update.Type)
                {
                    case SetOperationType.Add:
                        yield return SetOperation<TOut>.OnAdd(_func(update.Item));
                        break;

                    case SetOperationType.Remove:
                        yield return SetOperation<TOut>.OnRemove(_func(update.Item));
                        break;

                    case SetOperationType.Clear:
                        yield return SetOperation<TOut>.OnClear();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}