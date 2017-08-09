using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Structures;
using ObservableData.Structures.Utils;

namespace ObservableData.Querying.Utils.Adapters
{
    internal static class UpdateExtensions
    {
        private sealed class UpdateAdapter<T, TAdaptee> : IUpdate<T>
        {
            [NotNull] private readonly IUpdate<TAdaptee> _adaptee;
            [NotNull] private readonly Func<IEnumerable<TAdaptee>, IEnumerable<T>> _enumerate;

            public UpdateAdapter(
                [NotNull] IUpdate<TAdaptee> adaptee,
                [NotNull] Func<IEnumerable<TAdaptee>, IEnumerable<T>> enumerate)
            {
                _adaptee = adaptee;
                _enumerate = enumerate;
            }

            public IEnumerable<T> Operations()
            {
                return _enumerate(_adaptee.Operations()).NotNull();
            }

            public void Lock() => _adaptee.Lock();
        }

        private sealed class UpdateAdapterWithLock<T, TAdaptee> : IUpdate<T>
        {
            [NotNull] private readonly IUpdate<TAdaptee> _adaptee;
            [NotNull] private readonly Func<IEnumerable<TAdaptee>, IEnumerable<T>> _enumerate;


            [CanBeNull] private IEnumerable<T> _locked;
            private ThreadId _threadId;

            public UpdateAdapterWithLock(
                [NotNull] IUpdate<TAdaptee> adaptee,
                [NotNull] Func<IEnumerable<TAdaptee>, IEnumerable<T>> enumerate)
            {
                _adaptee = adaptee;
                _enumerate = enumerate;
                _threadId = ThreadId.FromCurrent();
            }

            public void Lock()
            {
                if (_locked != null) return;
                _threadId.CheckIsCurrent();
                _locked = this.Enumerate().ToArray();
            }

            public IEnumerable<T> Operations()
            {
                if (_locked != null) return _locked;
                _threadId.CheckIsCurrent();
                return this.Enumerate();
            }

            [NotNull]
            private IEnumerable<T> Enumerate()
            {
                return _enumerate(_adaptee.Operations()).NotNull();
            }
        }

        public static IUpdate<T> Adapt<T, TAdaptee>(
            [NotNull] this IUpdate<TAdaptee> update,
            [NotNull] Func<IEnumerable<TAdaptee>, IEnumerable<T>> enumerate)
        {
            return new UpdateAdapter<T, TAdaptee>(update, enumerate);
        }

        public static IUpdate<T> AdaptWithLock<T, TAdaptee>(
            [NotNull] this IUpdate<TAdaptee> update,
            [NotNull] Func<IEnumerable<TAdaptee>, IEnumerable<T>> enumerate)
        {
            return new UpdateAdapterWithLock<T, TAdaptee>(update, enumerate);
        }
    }
}