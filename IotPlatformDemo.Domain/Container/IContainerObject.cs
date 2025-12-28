using System.Text.Json.Serialization;

namespace IotPlatformDemo.Domain.Container;

public interface IContainerObject
{
    [JsonIgnore] public string ContainerName { get; }
    [JsonIgnore] public string PartitionKey { get; }
}