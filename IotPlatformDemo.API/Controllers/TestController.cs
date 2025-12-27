using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Identity.Web.Resource;

namespace IotPlatformDemo.API.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private readonly RegistryManager _registryManager;

    public TestController(RegistryManager registryManager)
    {
        _registryManager = registryManager;

        var configId = Guid.NewGuid().ToString();
        
        Device newDevice = new Device(configId);
        var addedDevice = _registryManager.AddDeviceAsync(newDevice).GetAwaiter().GetResult();

        Console.WriteLine($"Device ID: {addedDevice.Id}");
        Console.WriteLine($"Device Key: {addedDevice.Authentication.SymmetricKey.PrimaryKey}");
    }

    [HttpGet]
    [RequiredScopeOrAppPermission(
        RequiredScopesConfigurationKey = "AzureAD:Scopes:Read",
        RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:Read"
    )]
    public IEnumerable<TestData> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new TestData
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
