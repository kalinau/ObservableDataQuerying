﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils
{
    public struct ItemCounter<T>
    {
        private readonly uint _count;
        private readonly T _item;

        public ItemCounter(T item, uint count)
        {
            _count = count;
            _item = item;
        }

        public T Item => _item;

        public uint Count => _count;
    }

    public static class ItemCounterExtensions
    {
        public static IEnumerable<T> Enumerate<T>([NotNull] this IEnumerable<ItemCounter<T>> counters)
        {
            foreach (var counter in counters)
            {
                for (int i = 0; i < counter.Count; i++)
                {
                    yield return counter.Item;
                }
            }
        }

        public static bool TryIncreaseCount<TKey, TValue>([NotNull] this Dictionary<TKey, ItemCounter<TValue>> map, TKey key)
        {
            ItemCounter<TValue> existing;
            if (map.TryGetValue(key, out existing))
            {
                map[key] = new ItemCounter<TValue>(existing.Item, existing.Count + 1);
                return true;
            }
            return false;
        }

        public static uint DecreaseCount<TKey, TValue>([NotNull] this Dictionary<TKey, ItemCounter<TValue>> map, TKey key, out TValue currentValue)
        {
            ItemCounter<TValue> existing;
            if (map.TryGetValue(key, out existing))
            {
                throw new ArgumentOutOfRangeException();
            }
            currentValue = existing.Item;
            if (existing.Count > 1)
            {
                map[key] = new ItemCounter<TValue>(existing.Item, existing.Count - 1);
            }
            else
            {
                map.Remove(key);
            }
            return existing.Count;
        }
    }
}
