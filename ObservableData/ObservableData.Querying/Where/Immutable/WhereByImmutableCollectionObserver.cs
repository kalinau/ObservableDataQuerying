using System;
using JetBrains.Annotations;
using ObservableData.Querying.Core;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Where.Immutable
{
    public class WhereByImmutableCollectionObserver<T> : IObserver<IMutableEnumerable<SetOperation<T>>>
    {
        [NotNull] private readonly IObserver<IMutableEnumerable<SetOperation<T>>> _adaptee;
        [NotNull] private readonly Func<T, bool> _select;

        public WhereByImmutableCollectionObserver(
            [NotNull] IObserver<IMutableEnumerable<SetOperation<T>>> adaptee,
            [NotNull] Func<T, bool> select)
        {
            _adaptee = adaptee;
            _select = select;
        }

        public void OnCompleted() => _adaptee.OnCompleted();

        public void OnError(Exception error) => _adaptee.OnError(error);

        public void OnNext([NotNull] IMutableEnumerable<SetOperation<T>> value)
        {
            var update = new WhereByImmutableCollectionUpdate<T>(value, _select);
            _adaptee.OnNext(update);
        }
    }
}
