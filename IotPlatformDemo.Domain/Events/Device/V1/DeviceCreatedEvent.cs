using IotPlatformDemo.Domain.Events.Base.V1;

namespace IotPlatformDemo.Domain.Events.Device.V1;

public class DeviceCreatedEvent(string deviceId, string userId) : DeviceEvent(userId, Action.Create, deviceId)
{
}