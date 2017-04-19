﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Select.Immutable
{
    public sealed class SelectImmutableQuery<TIn, TOut> : IQuery<TOut>
    {
        [NotNull] private readonly IQuery<TIn> _previous;
        [NotNull] private readonly Func<TIn, TOut> _func;

        public SelectImmutableQuery([NotNull] IQuery<TIn> previous, [NotNull] Func<TIn, TOut> func)
        {
            _previous = previous;
            _func = func;
        }

        public void IgnoreEfficiency() { }

        public IDisposable Subscribe(IObserver<IUpdate<CollectionOperation<TOut>>> observer)
        {
            var map = new Dictionary<TIn, ItemCounter<TOut>>();
            var select = new SelectImmutableCollectionObserver<TIn, TOut>(observer, _func, map);

            return _previous.Subscribe(select);
        }

        public IDisposable Subscribe(IObserver<IUpdate<CollectionOperation<TOut>>> observer, out IReadOnlyCollection<TOut> mutableState)
        {
            var map = new Dictionary<TIn, ItemCounter<TOut>>();
            var select = new SelectImmutableCollectionObserver<TIn, TOut>(observer, _func, map);
            var subscription = _previous.Subscribe(select, out var previousState);
            mutableState = new SelectImmutableCollection<TIn, TOut>(previousState, map);

            return subscription;
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<TOut>>> observer)
        {
            var map = new Dictionary<TIn, ItemCounter<TOut>>();
            var select = new SelectImmutableListObserver<TIn,TOut>(observer, _func, map);

            return _previous.Subscribe(select);
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<TOut>>> observer, out IReadOnlyList<TOut> mutableState)
        {
            var map = new Dictionary<TIn, ItemCounter<TOut>>();
            var select = new SelectImmutableListObserver<TIn, TOut>(observer, _func, map);
            var subscription = _previous.Subscribe(select, out var previousState);
            mutableState = new SelectImmutableList<TIn, TOut>(previousState, map);

            return subscription;
        }
    }
}
