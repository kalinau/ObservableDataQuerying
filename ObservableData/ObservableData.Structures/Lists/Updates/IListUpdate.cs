using ObservableData.Querying.Core;

namespace ObservableData.Structures.Lists.Updates
{
    // ReSharper disable once PossibleInterfaceMemberAmbiguity
    internal interface IListUpdate<out T> : IUpdate<IListOperation<T>>, IUpdate<ICollectionOperation<T>>
    {
        
    }
}