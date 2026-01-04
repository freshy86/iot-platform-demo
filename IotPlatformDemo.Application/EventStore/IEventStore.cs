using IotPlatformDemo.Domain.Events.Base.V1;

namespace IotPlatformDemo.Application.EventStore;

public interface IEventStore
{
    Task Append(Event newObject);
}