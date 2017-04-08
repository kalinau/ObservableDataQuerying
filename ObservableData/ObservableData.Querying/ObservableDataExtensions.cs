using System;
using JetBrains.Annotations;
using ObservableData.Querying.Core;
using ObservableData.Querying.Select.Constant;
using ObservableData.Querying.Where.Immutable;

namespace ObservableData.Querying
{
    public static class ObservableDataExtensions
    {
        [NotNull]
        public static IObservableData<TIn> SelectImmutable<TIn, TOut>(
            [NotNull] this IObservableData<TIn> list, [NotNull] Func<TIn, TOut> func) =>
            throw new NotImplementedException();

        [NotNull]
        public static IObservableData<T> SelectMutable<T>(
            [NotNull] this IObservableData<T> list) =>
            throw new NotImplementedException();

        [NotNull]
        public static IObservableData<TOut> SelectConstant<TIn, TOut>(
            [NotNull] this IObservableData<TIn> data,
            [NotNull] Func<TIn, TOut> func) => 
            new SelectConstantData<TIn, TOut>(data, func);

        [NotNull]
        public static IObservableData<T> Where<T>(
            [NotNull] this IObservableData<T> data,
            [NotNull] Func<T, bool> func) => 
            new WhereByImmutableData<T>(data, func);
    }
}