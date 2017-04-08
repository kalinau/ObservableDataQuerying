using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Core
{
    public interface IObservableData<T>
    {
        void IgnoreEfficiency();

        [NotNull]
        IDisposable Subscribe([NotNull] IObserver<IUpdate<SetOperation<T>>> observer);

        [NotNull]
        IDisposable Subscribe([NotNull] IObserver<IUpdate<SetOperation<T>>> observer, out IReadOnlyCollection<T> mutableState);

        [NotNull]
        IDisposable Subscribe([NotNull] IObserver<IUpdate<ListOperation<T>>> observer);

        [NotNull]
        IDisposable Subscribe([NotNull] IObserver<IUpdate<ListOperation<T>>> observer, out IReadOnlyList<T> mutableState);
    }
}
