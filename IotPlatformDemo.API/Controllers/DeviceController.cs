using System.Security.Claims;
using IotPlatformDemo.Application.EventStore;
using IotPlatformDemo.Domain.Events.Device.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Identity.Web.Resource;

namespace IotPlatformDemo.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class DeviceController(
    RegistryManager registryManager,
    IHttpContextAccessor contextAccessor,
    IEventStore eventStore)
    : ControllerBase
{
    [HttpPost]
    [RequiredScopeOrAppPermission(
        RequiredScopesConfigurationKey = "AzureAD:Scopes:Write",
        RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:Write"
    )]
    public async Task<IActionResult> AddNewDevice()
    {
        var deviceId = Guid.NewGuid();
        var userId = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return BadRequest("User not found");
        }

        var iotDeviceId = $"{userId}-{deviceId}";
        //Device newDevice = new(iotDeviceId);
        //var addedDevice = await registryManager.AddDeviceAsync(newDevice);

        Console.WriteLine($"Added new IoT device with ID: {iotDeviceId}");
        await eventStore.Append(new DeviceCreatedEvent(iotDeviceId, userId));
        
        return Ok();
        //Console.WriteLine($"Device Key: {addedDevice.Authentication.SymmetricKey.PrimaryKey}");
    }
}