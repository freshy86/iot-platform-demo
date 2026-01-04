using IotPlatformDemo.Domain.AggregateRoots.Device;
using IotPlatformDemo.Domain.Events.Device.V1;
using IotPlatformDemo.Functions.Events;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace IotPlatformDemo.Functions.EventHandler.Device;

public class DeviceEventHandlerFunctions(ILogger<DeviceEventHandlerFunctions> logger)
{
    [Function(nameof(Device_RunEventOrchestrator))]
    public async Task Device_RunEventOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context, DeviceEvent deviceEvent)
    {
        var aggregateRoot = await context.CallActivityAsync<DeviceAggregateRoot>(nameof(Device_UpdateAggregateRoot), deviceEvent);
        await context.CallActivityAsync(nameof(Device_UpdateMaterializedViews), aggregateRoot);
    }

    [Function(nameof(Device_UpdateAggregateRoot))]
    public DeviceAggregateRoot Device_UpdateAggregateRoot([ActivityTrigger] DeviceEvent deviceEvent,
        FunctionContext executionContext)
    {
        logger.LogInformation("Updating device aggregate root");
        return new DeviceAggregateRoot(deviceEvent.PartitionKey);
    }
    
    [Function(nameof(Device_UpdateMaterializedViews))]
    public void Device_UpdateMaterializedViews([ActivityTrigger] DeviceAggregateRoot aggregateRoot,
        FunctionContext executionContext)
    {
        logger.LogInformation("Updating device materialized views");
    }
}