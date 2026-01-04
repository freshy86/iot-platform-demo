using Newtonsoft.Json;

namespace IotPlatformDemo.Domain.AggregateRoots.Device;

public class DeviceAggregateRoot(string deviceId): AggregateRoot(deviceId)
{
    [JsonProperty] public string DeviceName { get; set; }
    [JsonProperty] public string DeviceId { get; set; }
    [JsonProperty] public string UserId { get; set; }
}