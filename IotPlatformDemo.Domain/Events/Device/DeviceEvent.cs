using Newtonsoft.Json;

namespace IotPlatformDemo.Domain.Events.Device;

public class DeviceEvent(string userId, Action action, string deviceId) : Event(userId, EventType.DeviceEvent, action, 
    partitionKey: deviceId)
{
    [JsonProperty] public string DeviceId { get; } = deviceId;
}