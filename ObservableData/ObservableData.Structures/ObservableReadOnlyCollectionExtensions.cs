using System;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Core;
using ObservableData.Structures.Updates;

namespace ObservableData.Structures
{
    public static class ObservableReadOnlyCollectionExtensions
    {
        [NotNull]
        public static IObservable<IUpdate<SetOperation<T>>> AsObservable<T>(
            [NotNull] this IObservableReadOnlyCollection<T> list) =>
            list.Updates.StartWith(new AddMutableSequenceToCollection<T>(list));
    }
}
