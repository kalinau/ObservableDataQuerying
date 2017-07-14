using System;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Structures.Collections.Updates;
using ObservableData.Structures.Utils;

namespace ObservableData.Structures
{
    public static class CollectionExtensions
    {
        [NotNull]
        public static IObservable<IUpdate<ICollectionOperation<T>>> AsObservable<T>(
            [NotNull] this IObservableReadOnlyCollection<T> list) =>
            list.Updates.StartWith(new CollectionInsertBatchOperation<T>(list)).NotNull();
    }
}
