using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application;

[JsonPolymorphic]
[JsonDerivedType(typeof(MemoCreatedIntegrationEvent), typeDiscriminator: nameof(MemoCreatedIntegrationEvent))]
public abstract class IntegrationEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();

    required public Guid CorrelationId { get; init; }

    public static IntegrationEvent JsonDeserialize(string json)
    {
        var integrationEvent = JsonSerializer.Deserialize<IntegrationEvent>(json);

        if (integrationEvent is null)
        {
            throw new InvalidOperationException($"could not deserialize integration event");
        }

        return integrationEvent;
    }

    public string JsonSerialize()
    {
        return JsonSerializer.Serialize<IntegrationEvent>(this);
    }
}