using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Identity.Web.Resource;

[Authorize]
[ApiController]
[Route("[controller]")]
public class DeviceController : ControllerBase
{
    private readonly RegistryManager _registryManager;

    private readonly IHttpContextAccessor _contextAccessor;

    public DeviceController(RegistryManager registryManager,
                            IHttpContextAccessor contextAccessor
                            )
    {
        _registryManager = registryManager;
        _contextAccessor = contextAccessor;
    }

    [HttpPost]
    [RequiredScopeOrAppPermission(
        RequiredScopesConfigurationKey = "AzureAD:Scopes:Write",
        RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:Write"
    )]
    public async Task<IActionResult> AddNewDevice()
    {
        var deviceId = Guid.NewGuid();
        var userId = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return BadRequest("User not found");
        }

        Device newDevice = new($"{userId}-{deviceId}");
        var addedDevice = await _registryManager.AddDeviceAsync(newDevice);

        Console.WriteLine($"Added new IoT device with ID: {addedDevice.Id}");
        return Ok();
        //Console.WriteLine($"Device Key: {addedDevice.Authentication.SymmetricKey.PrimaryKey}");
    }
}