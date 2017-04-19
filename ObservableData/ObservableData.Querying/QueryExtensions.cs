using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Compatibility;
using ObservableData.Querying.Select.Constant;
using ObservableData.Querying.Where.Immutable;

namespace ObservableData.Querying
{
    public static class QueryExtensions
    {
        [NotNull]
        public static IQuery<TIn> SelectImmutable<TIn, TOut>(
            [NotNull] this IQuery<TIn> list, [NotNull] Func<TIn, TOut> func) =>
            throw new NotImplementedException();

        [NotNull]
        public static IQuery<T> SelectMutable<T>(
            [NotNull] this IQuery<T> list) =>
            throw new NotImplementedException();

        [NotNull]
        public static IQuery<TOut> SelectConstant<TIn, TOut>(
            [NotNull] this IQuery<TIn> data,
            [NotNull] Func<TIn, TOut> func) => 
            new SelectConstantQuery<TIn, TOut>(data, func);

        [NotNull]
        public static IQuery<T> WhereImmutable<T>(
            [NotNull] this IQuery<T> data,
            [NotNull] Func<T, bool> func) => 
            new WhereByImmutableData<T>(data, func);

        [NotNull]
        public static IEnumerable<T> AsBindableList<T>(
            [NotNull] this IQuery<T> data) =>
            new QueryAsBindableListAdapter<T>(data);
    }
}