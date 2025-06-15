using MassTransit;
namespace SharedLibrary.Common.Event
{
    public sealed class EventBuffer : IEventBuffer, IEventUnitOfWork, IEventFlusher
    {
        private readonly List<object> _events = [];

        public void Add<T>(T @event) where T : class
            => _events.Add(@event);

        public IReadOnlyCollection<object> DequeueAll()
        {
            var copy = _events.ToArray();
            _events.Clear();
            return copy;
        }

        public async Task FlushAsync(IPublishEndpoint bus,
                                     CancellationToken ct = default)
        {
            foreach (var e in DequeueAll()){
                Console.WriteLine("========================================");
                Console.WriteLine("EventBuffer: Flushing event");
                Console.WriteLine("Event: {e}");
                Console.WriteLine("========================================");
                await bus.Publish(e, ct);
            }
        }
    }
}