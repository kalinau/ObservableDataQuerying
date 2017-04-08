using System;

namespace ObservableData.Querying.Utils.Efficiency
{
    public class InefficientUsageException : Exception
    {
        public InefficientUsageException()
        {
        }

        public InefficientUsageException(string message) : base(message)
        {
        }

        public InefficientUsageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
