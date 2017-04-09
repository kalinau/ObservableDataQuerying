using System;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying;
using ObservableData.Querying.Core;
using ObservableData.Structures.Lists.Updates;
using ObservableData.Structures.Utils;

namespace ObservableData.Structures
{
    public static class ListExtensions
    {
        [NotNull]
        public static IObservable<IUpdate<IListOperation<T>>> AsObservable<T>(
            [NotNull] this IObservableReadOnlyList<T> list) =>
            list.Updates.StartWith(new ListInsertBatchOperation<T>(list, 0));

        [NotNull]
        public static IObservableData<T> AsData<T>(
            [NotNull] this IObservableReadOnlyList<T> list) => 
            new ListToDataAdapter<T>(list);

        [NotNull]
        public static IObservableData<TIn> SelectImmutable<TIn, TOut>(
            [NotNull] this IObservableReadOnlyList<TIn> list,
            [NotNull] Func<TIn, TOut> func) =>
            list.AsData().SelectImmutable(func);

        [NotNull]
        public static IObservableData<TOut> SelectConstant<TIn, TOut>(
            [NotNull] this IObservableReadOnlyList<TIn> list,
            [NotNull] Func<TIn, TOut> func) =>
            list.AsData().SelectConstant(func);

        [NotNull]
        public static IObservableData<TIn> Where<TIn, TCriteria>(
            [NotNull] this IObservableReadOnlyList<TIn> list,
            [NotNull] Func<TIn, bool> func) =>
            list.AsData().Where(func);
    }
}
