using System;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils
{
    public struct ThreadId
    {
        private readonly int? _threadId;

        public ThreadId(int threadId)
        {
            _threadId = threadId;
        }

        public static ThreadId FromCurrent()
        {
            return new ThreadId(Environment.CurrentManagedThreadId);
        }

        [Pure]
        public void CheckIsCurrent()
        {
            if (_threadId != Environment.CurrentManagedThreadId)
            {
                throw new InvalidOperationException("incorrect thread");
            }
        }
    }
}
