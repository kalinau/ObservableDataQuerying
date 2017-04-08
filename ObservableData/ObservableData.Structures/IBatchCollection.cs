using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Structures
{
    public interface IBatchCollection<T> : ICollection<T>
    {
        [NotNull]
        IDisposable StartBatchUpdate();

        void Reset([NotNull, InstantHandle] IEnumerable<T> items);

        void Add([NotNull, InstantHandle] IEnumerable<T> items);
    }
}