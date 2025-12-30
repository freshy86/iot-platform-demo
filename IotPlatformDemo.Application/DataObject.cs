using Newtonsoft.Json;

namespace IotPlatformDemo.Application;

public class DataObject<T>(string id, string partitionKey, T data)
{
    [JsonProperty] public string Id { get; } = id;
    [JsonProperty] public string PartitionKey = partitionKey;
    [JsonProperty] public T Data { get; } = data;
    [JsonProperty("_etag")] public string? Etag { get; set; }
    [JsonProperty] public int Ttl => -1;
}