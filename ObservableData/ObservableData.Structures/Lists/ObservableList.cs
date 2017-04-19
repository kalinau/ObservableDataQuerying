using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying;
using ObservableData.Structures.Lists.Updates;

namespace ObservableData.Structures.Lists
{
    public class ObservableList<T> : IObservableList<T>
    {
        [NotNull] private readonly List<T> _list;
        [NotNull] private readonly ListUpdatesAggregator<T> _subject = new ListUpdatesAggregator<T>();

        public ObservableList()
        {
            _list = new List<T>();
        }

        public ObservableList(IEnumerable<T> items)
        {
            _list = new List<T>(items);
        }

        public int Count => _list.Count;

        IObservable<IUpdate<IListOperation<T>>> IObservableReadOnlyList<T>.Updates => _subject;

        IObservable<IUpdate<ICollectionOperation<T>>> IObservableReadOnlyCollection<T>.Updates => _subject;

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get { return _list[index]; }
            set
            {
                T changedItem = _list[index];
                _list[index] = value;
                _subject.OnReplace(value, changedItem, index);
            }
        }
        

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(T item) => _list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public int IndexOf(T item) => _list.IndexOf(item);

        public IDisposable StartBatchUpdate() => _subject.StartBatchUpdate();

        public void Add(T item)
        {
            var index = _list.Count;
            _list.Add(item);
            _subject.OnAdd(item, index);
        }

        public void Clear()
        {
            if (_list.Count == 0) return;
            _list.Clear();
            _subject.OnReset(null);
        }

        public bool Remove(T item)
        {
            int index = _list.IndexOf(item);

            if (index >= 0)
            {
                this.RemoveAt(index);
                return true;
            }
            return false;
        }

        public void Move(int from, int to)
        {
            ListIndex.Check(from, this.Count);
            ListIndex.Check(to, this.Count);
            if (from == to) return;

            T item = _list[from];
            _list.RemoveAt(from);
            _list.Insert(to, item);
            _subject.OnMove(item, from, to);
        }


        public void Insert(int index, T item)
        {
            ListIndex.Check(index, this.Count + 1);
            _list.Insert(index, item);
            _subject.OnAdd(item, index);
        }

        public void RemoveAt(int index)
        {
            ListIndex.Check(index, _list.Count);
            T item = _list[index];
            _list.RemoveAt(index);
            _subject.OnRemove(item, index);
        }

        public void Reset(IEnumerable<T> items)
        {
            if (_list.Count == 0)
            {
                _list.AddRange(items);
                _subject.OnAdd(items, 0);
            }
            else
            {
                _list.Clear();
                _list.AddRange(items);
                _subject.OnReset(items);
            }
        }

        public void Add(IEnumerable<T> items)
        {
            var index = _list.Count;
            _list.AddRange(items);
            _subject.OnAdd(items, index);
        }
    }
}
