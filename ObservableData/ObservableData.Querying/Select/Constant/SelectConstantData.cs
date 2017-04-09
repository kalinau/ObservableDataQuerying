using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Core;
using ObservableData.Querying.Utils.Adapters;

namespace ObservableData.Querying.Select.Constant
{
    public sealed class SelectConstantData<TIn, TOut> : IObservableData<TOut>
    {
        [NotNull] private readonly IObservableData<TIn> _previous;
        [NotNull] private readonly Func<TIn, TOut> _func;

        public SelectConstantData([NotNull] IObservableData<TIn> previous, [NotNull] Func<TIn, TOut> func)
        {
            _previous = previous;
            _func = func;
        }

        public void IgnoreEfficiency() { }

        public IDisposable Subscribe(IObserver<IUpdate<CollectionOperation<TOut>>> observer)
        {
            var adapter = new CollectionObserverAdater(observer, _func);
            return _previous.Subscribe(adapter);
        }

        public IDisposable Subscribe(IObserver<IUpdate<CollectionOperation<TOut>>> observer, out IReadOnlyCollection<TOut> mutableState)
        {
            var adapter = new CollectionObserverAdater(observer, _func);

            var result = _previous.Subscribe(adapter, out var previousState);
            mutableState = new CollectionAdapter(previousState, _func);
            return result;
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<TOut>>> observer)
        {
            var adapter = new ListObserverAdater(observer, _func);
            return _previous.Subscribe(adapter);
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<TOut>>> observer, out IReadOnlyList<TOut> mutableState)
        {
            var adapter = new ListObserverAdater(observer, this._func);

            var result = _previous.Subscribe(adapter, out var previousState);
            mutableState = new ListAdapter(previousState, _func);
            return result;
        }


        private sealed class ListObserverAdater : ListObserverAdapter<TIn, TOut>
        {
            [NotNull] private readonly Func<TIn, TOut> _select;

            public ListObserverAdater([NotNull] IObserver<IUpdate<ListOperation<TOut>>> adaptee, [NotNull] Func<TIn, TOut> select) : base(adaptee)
            {
                _select = select;
            }

            protected override IUpdate<ListOperation<TOut>> HandleValue([NotNull] IUpdate<ListOperation<TIn>> value) =>
                new SelectConstantListUpdate<TIn,TOut>(value, _select);
        }

        private sealed class CollectionObserverAdater : CollectionObserverAdapter<TIn, TOut>
        {
            [NotNull] private readonly Func<TIn, TOut> _select;

            public CollectionObserverAdater([NotNull] IObserver<IUpdate<CollectionOperation<TOut>>> adaptee, [NotNull] Func<TIn, TOut> @select) : base(adaptee)
            {
                _select = select;
            }

            protected override IUpdate<CollectionOperation<TOut>> HandleValue([NotNull] IUpdate<CollectionOperation<TIn>> value) =>
                new SelectConstantCollectionUpdate<TIn,TOut>(value, _select);
        }

        private sealed class CollectionAdapter : CollectionAdapter<TIn, TOut>
        {
            [NotNull] private readonly Func<TIn, TOut> _select;

            public CollectionAdapter([NotNull] IReadOnlyCollection<TIn> source, [NotNull] Func<TIn, TOut> select) : base(source)
            {
                _select = select;
            }

            protected override IEnumerable<TOut> Enumerate(IEnumerable<TIn> source) => source.Select(_select);
        }

        private sealed class ListAdapter : ListAdapter<TIn, TOut>
        {
            [NotNull] private readonly Func<TIn,TOut> _select;

            public ListAdapter([NotNull] IReadOnlyList<TIn> source, [NotNull] Func<TIn, TOut> select) : base(source)
            {
                _select = select;
            }

            protected override IEnumerable<TOut> Enumerate(IEnumerable<TIn> source) => source.Select(_select);

            protected override TOut Select(TIn item) => _select(item);
        }
    }
}
