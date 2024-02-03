using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application;

[JsonPolymorphic]
[JsonDerivedType(typeof(MemoCreatedIntegrationEvent))]
public abstract class IntegrationEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    abstract public string Type { get; }

    public string JsonSerialize()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions());
    }

    public static IntegrationEvent JsonDeserialize(string json, string type)
    {
        var concreteType = GetEventTypeByName(type);
        var integrationEvent = JsonSerializer.Deserialize(json, concreteType) as IntegrationEvent;

        if (integrationEvent is null)
        {
            throw new InvalidOperationException($"could not deserialize integration event to type {type}");
        }

        return integrationEvent;
    }

    private static Type GetEventTypeByName(string type)
    {
        return type switch
        {
            nameof(MemoCreatedIntegrationEvent) => typeof(MemoCreatedIntegrationEvent),
            _ => throw new InvalidOperationException("unknown integration event type encountered")
        };
    }
}