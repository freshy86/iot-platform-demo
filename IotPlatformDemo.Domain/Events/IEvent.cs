using System;
using IotPlatformDemo.Domain.Container;

namespace IotPlatformDemo.Domain.Events;

public interface IEvent
{
    public string PartitionKey { get; }
    public Guid Id { get; }
    public Action Action { get; }
    public EventType Type { get; }
}