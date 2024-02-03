using System;
using System.Text.Json.Serialization;

namespace Application;

[JsonPolymorphic]
[JsonDerivedType(typeof(MemoCreatedIntegrationEvent))]
public abstract class IntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    abstract public string Type { get; }
    abstract public uint Version { get; }
}