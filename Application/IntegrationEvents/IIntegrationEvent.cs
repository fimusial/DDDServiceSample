using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application;

[JsonPolymorphic]
[JsonDerivedType(typeof(MemoCreatedIntegrationEvent), typeDiscriminator: nameof(MemoCreatedIntegrationEvent))]
public abstract class IntegrationEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string JsonSerialize()
    {
        return JsonSerializer.Serialize<IntegrationEvent>(this);
    }

    public static IntegrationEvent JsonDeserialize(string json)
    {
        var integrationEvent = JsonSerializer.Deserialize<IntegrationEvent>(json);

        if (integrationEvent is null)
        {
            throw new InvalidOperationException($"could not deserialize integration event");
        }

        return integrationEvent;
    }
}