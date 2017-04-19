﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters;
using ObservableData.Querying.Utils.Efficiency;

namespace ObservableData.Querying.Where.Immutable
{
    public sealed class WhereByImmutableData<T> : IQuery<T>
    {
        [NotNull] private readonly IQuery<T> _previous;
        [NotNull] private readonly Func<T, bool> _func;
        private bool _isWarningDisabled;

        public WhereByImmutableData([NotNull] IQuery<T> previous, [NotNull] Func<T, bool> func)
        {
            _previous = previous;
            _func = func;
        }

        public void IgnoreEfficiency()
        {
            _isWarningDisabled = true;
        }

        //public IDisposable Subscribe(IObserver<IUpdate<CollectionOperation<T>>> observer)
        //{
        //    var adapter = new CollectionObserverAdater(observer, _func);
        //    return _previous.Subscribe(adapter);
        //}

        public IDisposable Subscribe(IObserver<IUpdate<CollectionOperation<T>>> observer, out IReadOnlyCollection<T> mutableState)
        {
            var adapter = new CollectionObserverAdater(observer, _func);

            var result = _previous.Subscribe(adapter, out var previousState);
            mutableState = new CollectionAdapter(previousState, _func);
            return result;
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<T>>> observer)
        {
            Inneficient.Assert(_isWarningDisabled);
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<T>>> observer, out IReadOnlyList<T> mutableState)
        {
            Inneficient.Assert(_isWarningDisabled);
            throw new NotImplementedException();
        }

        private sealed class CollectionObserverAdater : CollectionObserverAdapter<T, T>
        {
            [NotNull] private readonly Func<T, bool> _func;

            public CollectionObserverAdater([NotNull] IObserver<IUpdate<CollectionOperation<T>>> adaptee, [NotNull] Func<T, bool> func) : base(adaptee)
            {
                _func = func;
            }

            protected override IUpdate<CollectionOperation<T>> HandleValue([NotNull] IUpdate<CollectionOperation<T>> value)
            {
                return new WhereByImmutableCollectionUpdate<T>(value, _func);
            }
        }

        private sealed class CollectionAdapter : CollectionAdapter<T, T>
        {
            [NotNull] private readonly Func<T, bool> _func;

            public CollectionAdapter([NotNull] IReadOnlyCollection<T> source, [NotNull] Func<T, bool> func) : base(source)
            {
                _func = func;
            }

            protected override IEnumerable<T> Enumerate(IEnumerable<T> source)
            {
                return source.Where(_func);
            }
        }
    }
}
