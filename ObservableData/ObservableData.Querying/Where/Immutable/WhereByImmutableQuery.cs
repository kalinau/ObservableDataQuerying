using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters;
using ObservableData.Querying.Utils.Efficiency;
using ObservableData.Structures;

namespace ObservableData.Querying.Where.Immutable
{
    public sealed class WhereByImmutableQuery<T> : IQuery<T>
    {
        [NotNull] private readonly IQuery<T> _previous;
        [NotNull] private readonly Func<T, bool> _func;
        private bool _isWarningDisabled;

        public WhereByImmutableQuery([NotNull] IQuery<T> previous, [NotNull] Func<T, bool> func)
        {
            _previous = previous;
            _func = func;
        }

        public void IgnoreEfficiency()
        {
            _isWarningDisabled = true;
        }

        public IDisposable Subscribe(IObserver<IUpdate<CollectionOperation<T>>> observer)
        {
            var adapter = observer.Wrap(this.Adapt);
            return _previous.Subscribe(adapter);
        }

        public IDisposable Subscribe(IObserver<IUpdate<CollectionOperation<T>>> observer, out IReadOnlyCollection<T> mutableState)
        {
            var adapter = observer.Wrap(this.Adapt);

            IReadOnlyCollection<T> previousState;
            var result = _previous.Subscribe(adapter, out previousState);
            mutableState = previousState.WhereForCollection(_func);
            return result;
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<T>>> observer)
        {
            Inneficient.Assert(_isWarningDisabled);
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<IUpdate<ListOperation<T>>> observer, out IReadOnlyList<T> mutableState)
        {
            Inneficient.Assert(_isWarningDisabled);
            throw new NotImplementedException();
        }

        private IUpdate<CollectionOperation<T>> Adapt([CanBeNull] IUpdate<CollectionOperation<T>> e)
        {
            if (e == null) return null;

            IEnumerable<CollectionOperation<T>> Enumerate(IEnumerable<CollectionOperation<T>> updates)
            {
                if (updates == null) yield break;

                foreach (var update in updates)
                {
                    if (update.Type == CollectionOperationType.Clear)
                    {
                        yield return update;
                    }
                    else if (_func.Invoke(update.Item))
                    {
                        yield return update;
                    }
                }
            }

            return e.Adapt(Enumerate);
        }
    }
}