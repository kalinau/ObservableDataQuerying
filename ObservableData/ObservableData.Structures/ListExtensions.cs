using System;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Structures.Lists.Updates;

namespace ObservableData.Structures
{
    public static class ListExtensions
    {
        [NotNull]
        public static IObservable<IUpdate<IListOperation<T>>> AsObservable<T>(
            [NotNull] this IObservableReadOnlyList<T> list) =>
            list.Updates.StartWith(new ListInsertBatchOperation<T>(list, 0));
    }
}
