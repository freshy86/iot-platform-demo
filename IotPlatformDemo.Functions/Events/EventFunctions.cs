using Azure.Messaging.ServiceBus;
using IotPlatformDemo.Application;
using IotPlatformDemo.Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IotPlatformDemo.Functions.Events;

public class EventFunctions(ILogger<EventFunctions> logger, IServiceHubContext serviceHubContext)
{
    [Function(nameof(EventToServiceBusForwarding))]
    public async Task EventToServiceBusForwarding([CosmosDBTrigger(
            databaseName: "iot_demo_write",
            containerName: "events",
            Connection = "CosmosDb",
            LeaseContainerName = "leases",
            LeaseContainerPrefix = $"events{LeaseContainerPrefixConstants.Extension}",
            CreateLeaseContainerIfNotExists = true)] List<Event> events,
        FunctionContext context)
    {
        logger.LogInformation("C# Cosmos DB trigger function processed {count} documents.", events?.Count ?? 0);
        
        Dictionary<string, List<ServiceBusMessage>> serviceBusMessages = new();
        
        if (events is not null && events.Count != 0)
        {
            foreach (var e in events)
            {
                logger.LogInformation("Data: {desc}", e);
                
                var serviceBusMessage = new ServiceBusMessage(JsonConvert.SerializeObject(e));
                {

                };
                //await serviceHubContext.Clients.User(e.UserId).SendAsync("notification", "system", $"Event received: {e.Type}, {e.Action} for user: {e.UserId}");
            }
        }
    }
}