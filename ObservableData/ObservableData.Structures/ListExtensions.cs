using System;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Structures.Lists.Updates;
using ObservableData.Structures.Utils;

namespace ObservableData.Structures
{
    public static class ListExtensions
    {
        [NotNull]
        public static IObservable<IUpdate<IListOperation<T>>> AsObservable<T>(
            [NotNull] this IObservableReadOnlyList<T> list) =>
            list.Updates.StartWith(new ListInsertBatchOperation<T>(list, 0)).NotNull();

        public static void Do()
        {

        }
    }
}
