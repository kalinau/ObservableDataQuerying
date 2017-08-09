using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;
using ObservableData.Querying.Utils.Adapters;
using ObservableData.Structures;

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

            var adapter = observer.Wrap(
                update => this.Adapt(update, map),
                default(IUpdate<CollectionOperation<TIn>>));

            return _previous.Subscribe(adapter);
        }

        public IDisposable Subscribe(IObserver<IUpdate<CollectionOperation<TOut>>> observer, out IReadOnlyCollection<TOut> mutableState)
        {
            var map = new Dictionary<TIn, ItemCounter<TOut>>();

            var adapter = observer.Wrap(
                update => this.Adapt(update, map),
                default(IUpdate<CollectionOperation<TIn>>));

            IReadOnlyCollection<TIn> previousState;
            var subscription = _previous.Subscribe(adapter, out previousState);
            this.FillMap(map, previousState);
            mutableState = new SelectImmutableCollection<TIn, TOut>(previousState, map);

            return subscription;
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<TOut>>> observer)
        {
            var map = new Dictionary<TIn, ItemCounter<TOut>>();

            var adapter = observer.Wrap(
                update => this.Adapt(update, map),
                default(IUpdate<ListOperation<TIn>>));

            return _previous.Subscribe(adapter);
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<TOut>>> observer, out IReadOnlyList<TOut> mutableState)
        {
            var map = new Dictionary<TIn, ItemCounter<TOut>>();

            var adapter = observer.Wrap(
                update => this.Adapt(update, map),
                default(IUpdate<ListOperation<TIn>>));

            IReadOnlyList<TIn> previousState;
            var subscription = _previous.Subscribe(adapter, out previousState);
            this.FillMap(map, previousState);
            mutableState = new SelectImmutableList<TIn, TOut>(previousState, map);

            return subscription;
        }

        private void FillMap(
            [NotNull] Dictionary<TIn, ItemCounter<TOut>> map,
            [NotNull] IReadOnlyCollection<TIn> state)
        {
            foreach (var item in state)
            {
                if (!map.TryIncreaseCount(item))
                {
                    map[item] = new ItemCounter<TOut>(_func(item), 1);
                }
            }
        }

        private IUpdate<ListOperation<TOut>> Adapt(
            [CanBeNull] IUpdate<ListOperation<TIn>> e,
            [NotNull] Dictionary<TIn, ItemCounter<TOut>> map)
        {
            if (e == null) return null;

            Dictionary<TIn, TOut> removedMap = null;

            void OnAdd(TIn item)
            {
                if (!map.TryIncreaseCount(item))
                {
                    var result = default(TOut);
                    if (removedMap?.TryGetValue(item, out result) != true)
                    {
                        result = _func(item);
                    }
                    map[item] = new ItemCounter<TOut>(result, 1);
                }
            }

            void OnRemove(TIn item)
            {
                TOut removed;
                if (map.DecreaseCount(item, out removed) <= 1)
                {
                    if (removedMap == null)
                    {
                        removedMap = new Dictionary<TIn, TOut>();
                    }
                    removedMap[item] = removed;
                }
            }

            foreach (var update in e.Operations())
            {
                switch (update.Type)
                {
                    case ListOperationType.Add:
                        OnAdd(update.Item);
                        break;

                    case ListOperationType.Remove:
                        OnRemove(update.Item);
                        break;

                    case ListOperationType.Move:
                        break;

                    case ListOperationType.Replace:
                        OnRemove(update.Item);
                        OnAdd(update.Item);
                        break;

                    case ListOperationType.Clear:
                        map.Clear();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            IEnumerable<ListOperation<TOut>> Enumerate(IEnumerable<ListOperation<TIn>> updates)
            {
                if (updates == null) yield break;

                foreach (var update in updates)
                {
                    switch (update.Type)
                    {
                        case ListOperationType.Add:
                            yield return ListOperation<TOut>.OnAdd(
                                map.Get(update.Item, removedMap),
                                update.Index);
                            break;

                        case ListOperationType.Remove:
                            yield return ListOperation<TOut>.OnRemove(
                                map.Get(update.Item, removedMap),
                                update.Index);
                            break;

                        case ListOperationType.Move:
                            yield return ListOperation<TOut>.OnMove(
                                map.Get(update.Item, removedMap),
                                update.Index,
                                update.OriginalIndex);
                            break;

                        case ListOperationType.Replace:
                            yield return ListOperation<TOut>.OnReplace(
                                map.Get(update.Item, removedMap),
                                map.Get(update.ChangedItem, removedMap),
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

            return e.AdaptWithLock(Enumerate);
        }

        private IUpdate<CollectionOperation<TOut>> Adapt(
            [CanBeNull] IUpdate<CollectionOperation<TIn>> e,
            [NotNull] Dictionary<TIn, ItemCounter<TOut>> map)
        {
            if (e == null) return null;

            Dictionary<TIn, TOut> removedMap = null;

            void OnAdd(TIn item)
            {
                if (!map.TryIncreaseCount(item))
                {
                    var result = default(TOut);
                    if (removedMap?.TryGetValue(item, out result) != true)
                    {
                        result = _func(item);
                    }
                    map[item] = new ItemCounter<TOut>(result, 1);
                }
            }

            void OnRemove(TIn item)
            {
                if (map.DecreaseCount(item, out var removed) <= 1)
                {
                    if (removedMap == null)
                    {
                        removedMap = new Dictionary<TIn, TOut>();
                    }
                    removedMap[item] = removed;
                }
            }

            foreach (var update in e.Operations())
            {
                switch (update.Type)
                {
                    case CollectionOperationType.Add:
                        OnAdd(update.Item);
                        break;

                    case CollectionOperationType.Remove:
                        OnRemove(update.Item);
                        break;

                    case CollectionOperationType.Clear:
                        map.Clear();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            IEnumerable<CollectionOperation<TOut>> Enumerate(IEnumerable<CollectionOperation<TIn>> updates)
            {
                if (updates == null) yield break;

                foreach (var update in updates)
                {
                    switch (update.Type)
                    {
                        case CollectionOperationType.Add:
                            yield return CollectionOperation<TOut>.OnAdd(map.Get(update.Item, removedMap));
                            break;

                        case CollectionOperationType.Remove:
                            yield return CollectionOperation<TOut>.OnRemove(map.Get(update.Item, removedMap));
                            break;

                        case CollectionOperationType.Clear:
                            yield return CollectionOperation<TOut>.OnClear();
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return e.AdaptWithLock(Enumerate);
        }
    }
}
