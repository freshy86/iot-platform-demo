using System.Text;
using Azure.Messaging.ServiceBus;
using IotPlatformDemo.Domain.Events.Base.V1;
using IotPlatformDemo.Domain.Events.Device.V1;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IotPlatformDemo.Functions.Events;

public class EventConsumerFunctions(ILogger<EventConsumerFunctions> logger)
{
    [Function(nameof(Event_StartOrchestration))]
    public async Task Event_StartOrchestration(
        [ServiceBusTrigger("events", "events-subscription", Connection = "ServiceBus", 
            IsBatched = false, IsSessionsEnabled = true)] ServiceBusReceivedMessage message,
        [DurableClient] DurableTaskClient client, FunctionContext executionContext)
    {
        try
        {
            var eventsAssembly = typeof(Event).Assembly;
            var eventType = eventsAssembly.GetType($"{message.ApplicationProperties[nameof(Event.Version)]}", true)!;
            var e = (JsonConvert.DeserializeObject(Encoding.UTF8.GetString(message.Body), eventType) as Event)!;
        
            logger.LogInformation("Event received: {e}", e);

            if (e is DeviceCreatedEvent)
            {
                var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                    nameof(EventHandler.Device.DeviceEventHandlerFunctions.Device_RunEventOrchestrator), e);
                logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);
            }

            //await serviceHubContext.Clients.User(e.UserId).SendAsync("notification", "system", $"Event received: {e.Type}, {e.Action} for user: {e.UserId}");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Orchestration by event could not be started.");
            throw;
        }
    }
}