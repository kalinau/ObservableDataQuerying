using System;

namespace ObservableData.Querying.Utils
{
    public class MutableUpdate
    {
        private readonly int? _threadId;

        protected MutableUpdate()
        {
            _threadId = Environment.CurrentManagedThreadId;
        }

        protected void CheckAccess()
        {
            if (_threadId != Environment.CurrentManagedThreadId)
            {
                if (_threadId == null)
                {
                    throw new ObjectDisposedException("object is diposed");
                }
                throw new InvalidOperationException("object is used from incorrect thread");
            }
        }
    }
}
