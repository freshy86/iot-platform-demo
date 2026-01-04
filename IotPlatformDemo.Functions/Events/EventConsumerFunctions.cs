using System.Text;
using Azure.Messaging.ServiceBus;
using IotPlatformDemo.Domain.Events.Base.V1;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IotPlatformDemo.Functions.Events;

public class EventConsumerFunctions(ILogger<EventConsumerFunctions> logger)
{
    [Function(nameof(EventConsumerFunctions))]
    public async Task<List<string>> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var outputs = new List<string>();

        // Replace name and input with values relevant for your Durable Functions Activity
        outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Tokyo"));
        outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Seattle"));
        outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "London"));

        // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
        return outputs;
    }

    [Function(nameof(SayHello))]
    public string SayHello([ActivityTrigger] string name, FunctionContext executionContext)
    {
        logger.LogInformation("Saying hello to {name}.", name);
        return $"Hello {name}!";
    }

    [Function(nameof(StartOrchestrationByEvent))]
    public async Task StartOrchestrationByEvent(
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

            var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(EventConsumerFunctions));

            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);
            //await serviceHubContext.Clients.User(e.UserId).SendAsync("notification", "system", $"Event received: {e.Type}, {e.Action} for user: {e.UserId}");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Orchestration by event could not be started.");
            throw;
        }
    }
}