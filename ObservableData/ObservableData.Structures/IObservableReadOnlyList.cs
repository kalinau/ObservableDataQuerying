using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Core;

namespace ObservableData.Structures
{
    public interface IObservableReadOnlyList<T> : IReadOnlyList<T>
    {
        new int Count { get; }

        [NotNull]
        IObservable<IUpdate<ListOperation<T>>> Updates { get; }
    }
}