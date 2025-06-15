using MassTransit;

namespace SharedLibrary.Common.Event
{
    public interface IEventFlusher
    {
        Task FlushAsync(IPublishEndpoint bus, CancellationToken ct = default);
    }
}