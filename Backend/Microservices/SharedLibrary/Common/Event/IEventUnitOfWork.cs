namespace SharedLibrary.Common.Event
{
    public interface IEventUnitOfWork
    {
        void Add<T>(T @event) where T : class;
    }
}