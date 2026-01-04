using IotPlatformDemo.Domain.Events.Base.V1;
using Newtonsoft.Json;

namespace IotPlatformDemo.Domain.Events.Device.V1;

public class DeviceEvent(string userId, Action action, string deviceId) : Event(userId, EventType.DeviceEvent, action, 
    partitionKey: deviceId)
{
    [JsonProperty] public string DeviceId { get; } = deviceId;
}