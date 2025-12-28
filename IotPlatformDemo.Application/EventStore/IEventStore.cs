using IotPlatformDemo.Domain.Events;

namespace IotPlatformDemo.Application.EventStore;

public interface IEventStore
{
    Task Append<T>(T newEvent) where T : IEvent;
}