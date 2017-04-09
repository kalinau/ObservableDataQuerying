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
        public static IQuery<T> AsQuery<T>(
            [NotNull] this IObservableReadOnlyList<T> list) => 
            new ListToQueryAdapter<T>(list);

        [NotNull]
        public static IQuery<TIn> SelectImmutable<TIn, TOut>(
            [NotNull] this IObservableReadOnlyList<TIn> list,
            [NotNull] Func<TIn, TOut> func) =>
            list.AsQuery().SelectImmutable(func);

        [NotNull]
        public static IQuery<TOut> SelectConstant<TIn, TOut>(
            [NotNull] this IObservableReadOnlyList<TIn> list,
            [NotNull] Func<TIn, TOut> func) =>
            list.AsQuery().SelectConstant(func);

        [NotNull]
        public static IQuery<TIn> Where<TIn, TCriteria>(
            [NotNull] this IObservableReadOnlyList<TIn> list,
            [NotNull] Func<TIn, bool> func) =>
            list.AsQuery().Where(func);
    }
}
