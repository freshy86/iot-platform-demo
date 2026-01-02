using Microsoft.AspNetCore.SignalR;

namespace IotPlatformDemo.Application.Notifications;

public class ClientNotificationHub: Hub<IClientNotificationHub>
{
    //Empty implementation, the client will not send messages to the server
}