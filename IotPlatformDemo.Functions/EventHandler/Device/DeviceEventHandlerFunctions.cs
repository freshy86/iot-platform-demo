using IotPlatformDemo.Domain.AggregateRoots.Device;
using IotPlatformDemo.Domain.Events.Base.V1;
using IotPlatformDemo.Domain.Events.Device.V1;
using IotPlatformDemo.Functions.General;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Action = System.Action;

namespace IotPlatformDemo.Functions.EventHandler.Device;

public class DeviceEventHandlerFunctions(ILogger<DeviceEventHandlerFunctions> logger, Container dataContainer)
{
    [Function(nameof(Device_RunEventOrchestrator))]
    public async Task Device_RunEventOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context, (string, string) userIdEventStringTuple)
    {
        var userId = userIdEventStringTuple.Item1;
        var eventAsString = userIdEventStringTuple.Item2;
        try
        {
            await context.CallActivityAsync(nameof(GeneralActivityFunctions.General_SignalOrchestrationStatusToFrontends), new OrchestrationStatus
            {
                Status = OrchestrationStatus.StatusCode.Start,
                UserId = userId,
                OrchestrationId = context.InstanceId
            });
        
            var options = TaskOptions.FromRetryPolicy(new RetryPolicy(
                maxNumberOfAttempts: 5,
                firstRetryInterval: TimeSpan.FromSeconds(1)));
            
            var aggregateRoot = await context.CallActivityAsync<DeviceAggregateRoot>(nameof(Device_UpdateAggregateRoot), 
                eventAsString, options);
            await context.CallActivityAsync(nameof(Device_UpdateMaterializedViews), aggregateRoot, options);
        
            await context.CallActivityAsync(nameof(GeneralActivityFunctions.General_SignalOrchestrationStatusToFrontends), new OrchestrationStatus
            {
                Status = OrchestrationStatus.StatusCode.Success,
                UserId = userId,
                OrchestrationId = context.InstanceId,
                Result = aggregateRoot.Id
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Orchestration failed.");
            await context.CallActivityAsync(nameof(GeneralActivityFunctions.General_SignalOrchestrationStatusToFrontends), new OrchestrationStatus
            {
                Status = OrchestrationStatus.StatusCode.Fail,
                UserId = userId,
                OrchestrationId = context.InstanceId
            });
            throw;
        }
    }

    [Function(nameof(Device_UpdateAggregateRoot))]
    public async Task<DeviceAggregateRoot> Device_UpdateAggregateRoot([ActivityTrigger] string eventAsString,
        FunctionContext executionContext)
    {
        var eventsAssembly = typeof(Event).Assembly;
        var jsonObject = (JsonConvert.DeserializeObject(eventAsString) as JObject)!;
        var eventType = eventsAssembly.GetType($"{jsonObject.GetValue("version")}", true)!;
        var receivedEvent = (jsonObject.ToObject(eventType) as DeviceEvent)!;

        DeviceAggregateRoot? aggregateRoot;
        if (receivedEvent.Action == Domain.Events.Base.V1.Action.Create)
        {
            aggregateRoot = new DeviceAggregateRoot(receivedEvent.PartitionKey);
        }
        else
        {
            aggregateRoot = await dataContainer.ReadItemAsync<DeviceAggregateRoot>(receivedEvent.DeviceId,
                new PartitionKey(receivedEvent.DeviceId), null, executionContext.CancellationToken);
        }

        if (aggregateRoot == null)
        {
            throw new Exception("Could not retrieve or create aggregate root");
        }
        
        logger.LogInformation("Updating device aggregate root");
        receivedEvent.Apply(aggregateRoot);

        var requestOptions = new ItemRequestOptions
        {
            IfMatchEtag = aggregateRoot.ETag
        };

        aggregateRoot = await dataContainer.UpsertItemAsync(aggregateRoot, new PartitionKey(receivedEvent.DeviceId), requestOptions,
            executionContext.CancellationToken);

        return aggregateRoot;
    }
    
    [Function(nameof(Device_UpdateMaterializedViews))]
    public void Device_UpdateMaterializedViews([ActivityTrigger] DeviceAggregateRoot aggregateRoot,
        FunctionContext executionContext)
    {
        //Task.Delay(3000).Wait();
        //throw new Exception("hmm!");
        logger.LogInformation("Updating device materialized views");
    }
}