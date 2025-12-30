using System;
using Newtonsoft.Json;

namespace IotPlatformDemo.Domain.Events;

public class Event(EventType type, Action action, string partitionKey) : IEvent
{ 
    [JsonProperty] public string PartitionKey { get; } = partitionKey;
    [JsonProperty] public Guid Id { get; } = Guid.NewGuid();
    [JsonProperty] public Action Action { get; } = action;
    [JsonProperty] public EventType Type { get; } = type;
}