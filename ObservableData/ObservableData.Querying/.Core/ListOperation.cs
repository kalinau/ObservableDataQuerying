﻿using JetBrains.Annotations;

namespace ObservableData.Querying.Core
{
    public struct ListOperation<T>
    {
        public ListOperation(ListOperationType type, T item, T changedItem, int index, int originalIndex)
        {
            Type = type;
            Item = item;
            ChangedItem = changedItem;
            Index = index;
            OriginalIndex = originalIndex;
        }

        public static ListOperation<T> OnClear()
        {
            return new ListOperation<T>(ListOperationType.Clear, default(T), default(T), default(int), default(int));
        }

        public static ListOperation<T> OnAdd(T item, int index)
        {
            return new ListOperation<T>(ListOperationType.Add, item, default(T), index, default(int));
        }

        public static ListOperation<T> OnRemove(T item, int index)
        {
            return new ListOperation<T>(ListOperationType.Remove, item, default(T), index, default(int));
        }

        public static ListOperation<T> OnMove(T item, int index, int fromIndex)
        {
            return new ListOperation<T>(ListOperationType.Move, item, default(T), index, fromIndex);
        }

        public static ListOperation<T> OnReplace(T item, T changedItem, int index)
        {
            return new ListOperation<T>(ListOperationType.Replace, item, changedItem, index, default(int));
        }

        public ListOperationType Type { get; }

        [CanBeNull]
        public T Item { get; }

        [CanBeNull]
        public T ChangedItem { get; }

        public int Index { get; }

        public int OriginalIndex { get; }
    }
}