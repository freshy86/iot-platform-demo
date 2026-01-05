using System;

namespace IotPlatformDemo.Domain.AggregateRoots;

public interface IAggregateRoot
{
    string Id { get; }
    string PartitionKey { get; }
    string ETag { get; }
}