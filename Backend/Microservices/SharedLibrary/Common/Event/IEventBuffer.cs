namespace SharedLibrary.Common.Event
{
    public interface IEventBuffer
    {
        void Add<T>(T @event) where T : class;
        IReadOnlyCollection<object> DequeueAll();
    }
}