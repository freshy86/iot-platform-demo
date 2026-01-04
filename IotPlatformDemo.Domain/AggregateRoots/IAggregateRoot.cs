using System;

namespace IotPlatformDemo.Domain.AggregateRoots;

public interface IAggregateRoot
{
    Guid Id { get; }
    string PartitionKey { get; }
}