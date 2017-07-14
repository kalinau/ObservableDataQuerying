namespace ObservableData.Structures.Lists.Updates
{
    // ReSharper disable once PossibleInterfaceMemberAmbiguity
    public interface IListUpdate<out T> : IUpdate<IListOperation<T>>, IUpdate<ICollectionOperation<T>>
    {
        
    }
}