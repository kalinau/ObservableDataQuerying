﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Core;

namespace ObservableData.Structures
{
    public interface IObservableReadOnlyList<out T> : IReadOnlyList<T>, IObservableReadOnlyCollection<T>
    {
        new int Count { get; }

        [NotNull]
        new IObservable<IUpdate<IListOperation<T>>> Updates { get; }
    }
}