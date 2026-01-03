using System;
using Newtonsoft.Json;

namespace IotPlatformDemo.Domain.Events.Device;

public class DeviceEvent(string userId, Action action, string deviceId) : Event(userId, EventType.DeviceEvent, action, 
    partitionKey: deviceId)
{
    public string DeviceId { get; } = deviceId;
    [JsonProperty] private DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
}