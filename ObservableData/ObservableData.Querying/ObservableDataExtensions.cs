using System;
using JetBrains.Annotations;
using ObservableData.Querying.Select.Constant;
using ObservableData.Querying.Where.Immutable;

namespace ObservableData.Querying
{
    public static class ObservableDataExtensions
    {
        [NotNull]
        public static IQuery<TIn> SelectImmutable<TIn, TOut>(
            [NotNull] this IQuery<TIn> list, [NotNull] Func<TIn, TOut> func)
        {
            throw new NotImplementedException();
        }

        [NotNull]
        public static IQuery<T> SelectMutable<T>(
            [NotNull] this IQuery<T> list)
        {
            throw new NotImplementedException();
        }

        [NotNull]
        public static IQuery<TOut> SelectConstant<TIn, TOut>(
            [NotNull] this IQuery<TIn> data,
            [NotNull] Func<TIn, TOut> func) =>
            new SelectConstantData<TIn, TOut>(data, func);

        [NotNull]
        public static IQuery<T> Where<T>(
            [NotNull] this IQuery<T> data,
            [NotNull] Func<T, bool> func) =>
            new WhereByImmutableData<T>(data, func);
    }
}