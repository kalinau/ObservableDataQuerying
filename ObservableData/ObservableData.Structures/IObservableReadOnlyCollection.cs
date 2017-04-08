using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Core;

namespace ObservableData.Structures
{
    public interface IObservableReadOnlyCollection<T> :IReadOnlyCollection<T>
    {
        [NotNull]
        IObservable<IUpdate<SetOperation<T>>> Updates { get; }
    }
}