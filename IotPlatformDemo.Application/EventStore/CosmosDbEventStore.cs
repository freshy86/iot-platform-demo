using System.Collections.Concurrent;
using IotPlatformDemo.Domain.Container;
using IotPlatformDemo.Domain.Events;
using Microsoft.Azure.Cosmos;

namespace IotPlatformDemo.Application.EventStore;

public class CosmosDbEventStore(CosmosClient cosmosClient, string databaseName) : IEventStore
{
    private readonly ConcurrentDictionary<string, Container> _containerRegistry = new();

    private Container GetContainerForObject(IContainerObject containerObject)
    {
        if (_containerRegistry.TryGetValue(containerObject.ContainerName, out var value)) return value;
        value = cosmosClient.GetContainer(
            databaseName, containerObject.ContainerName);
        _containerRegistry[containerObject.ContainerName] = value;
        return value;
    }
    
    public async Task Append<T>(T newEvent) where T : IEvent
    {
        var container = GetContainerForObject(newEvent);
        var response = await container.CreateItemAsync(new DataObject($"{newEvent.Id}",
            newEvent, "deviceEvent")).ConfigureAwait(false);
    }
}