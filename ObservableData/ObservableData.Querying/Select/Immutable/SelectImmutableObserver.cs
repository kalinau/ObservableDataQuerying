using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using ObservableData.Querying.Core;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Select.Immutable
{
    public sealed class SelectImmutableObserver<TIn, TOut> : IObserver<IUpdate<SetOperation<TIn>>>, IReadOnlyCollection<TOut>
    {
        [CanBeNull] private IObserver<IUpdate<SetOperation<TOut>>> _adaptee;
        [NotNull] private readonly Func<TIn, TOut> _func;
        [NotNull] private readonly Dictionary<TIn, ItemCounter<TOut>> _map = new Dictionary<TIn, ItemCounter<TOut>>();
        private int _count;

        public SelectImmutableObserver([NotNull] Func<TIn, TOut> func)
        {
            _func = func;
        }

        public int Count => _count;

        public void OnNext([NotNull] IUpdate<SetOperation<TIn>> e)
        {
            Dictionary<TIn, TOut> removedMap = null;
            foreach (var update in e.Operations())
            {
                switch (update.Type)
                {
                    case SetOperationType.Add:
                        _map.TryGetValue(update.Item, out var existing);
                        if (existing.Count > 0)
                        {
                            _map[update.Item] = new ItemCounter<TOut>(existing.Item, existing.Count + 1);
                        }
                        else
                        {
                            var item = Select(update.Item, removedMap);
                            _map[update.Item] = new ItemCounter<TOut>(item, 1);
                        }
                        _count++;
                        break;

                    case SetOperationType.Remove:
                        _map.TryGetValue(update.Item, out var removed);
                        if (removed.Count > 1)
                        {
                            _map[update.Item] = new ItemCounter<TOut>(removed.Item, removed.Count - 1);
                        }
                        else
                        {
                            if (removedMap == null)
                            {
                                removedMap = new Dictionary<TIn, TOut>();
                            }
                            removedMap[update.Item] = removed.Item;
                            _map.Remove(update.Item);
                        }
                        _count--;
                        break;

                    case SetOperationType.Clear:
                        _count = 0;
                        _map.Clear();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (_adaptee != null)
            {
                var updates = new SelectImmutableSetUpdate<TIn, TOut>(e, _map, removedMap);
                _adaptee.OnNext(updates);
            }
        }

        public void SetAdaptee([NotNull] IObserver<IUpdate<SetOperation<TOut>>> adaptee)
        {
            _adaptee = adaptee;
        }

        private TOut Select(TIn item, Dictionary<TIn, TOut> alreadyRemoved)
        {
            if (alreadyRemoved == null || !alreadyRemoved.TryGetValue(item, out var result))
            {
                result = _func(item);
            }
            return result;
        }

        public void OnCompleted()
        {
            _adaptee?.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _adaptee?.OnError(error);
        }


        [NotNull]
        private IEnumerable<TOut> Enumerate()
        {
            Debug.Assert(_map.Values != null);
            foreach (var counter in _map.Values)
            {
                for (int i = 0; i < counter.Count; i++)
                {
                    yield return counter.Item;
                }
            }
        }

        public IEnumerator<TOut> GetEnumerator() => this.Enumerate().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
