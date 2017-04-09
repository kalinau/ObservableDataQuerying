using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying
{
    public interface IUpdate<out T>
    {
        [NotNull, ItemNotNull]
        IEnumerable<T> Operations();

        void Lock();
    }
}