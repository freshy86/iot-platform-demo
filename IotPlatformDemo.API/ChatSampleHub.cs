using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Web.Resource;

namespace IotPlatformDemo.API;

[Authorize]
[RequiredScopeOrAppPermission(
    RequiredScopesConfigurationKey = "AzureAD:Scopes:Read",
    RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:Read"
)]
public class ChatSampleHub : Hub
{
    public Task BroadcastMessage(string name, string message) =>
        Clients.All.SendAsync("broadcastMessage", name, message);

    public Task Echo(string name, string message) =>
        Clients.Client(Context.ConnectionId)
            .SendAsync("echo", name, $"{message} (echo from server)");
}