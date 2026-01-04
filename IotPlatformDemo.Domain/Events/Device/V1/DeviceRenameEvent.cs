using IotPlatformDemo.Domain.Events.Base.V1;
using Newtonsoft.Json;

namespace IotPlatformDemo.Domain.Events.Device.V1;

public class DeviceRenameEvent(string deviceId, string userId, string newDeviceName): DeviceEvent(userId, Action.Update, deviceId)
{
    [JsonProperty] public string NewDeviceName { get; } = newDeviceName;
}