using Avro;
using Avro.Specific;

namespace LinkTracker.Scrapper.Contracts.Avro;

public class LinkUpdateEvent : ISpecificRecord
{
    public static Schema _SCHEMA = Schema.Parse(
        """
        {
            "type": "record",
            "name": "LinkUpdateEvent",
            "namespace": "LinkTracker.Scrapper.Contracts.Avro",
            "fields": [
                { "name": "eventId", "type": "string" },
                { "name": "url", "type": "string" },
                { "name": "description", "type": "string" },
                {
                    "name": "tgChatIds",
                    "type": {
                    "type": "array",
                    "items": "long"
                }
                }
            ]
        }
        """);

    public virtual Schema Schema => _SCHEMA;

    public string eventId { get; set; } = string.Empty;
    public string url { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public IList<long> tgChatIds { get; set; } = new List<long>();

    public virtual object Get(int fieldPos)
    {
        return fieldPos switch
        {
            0 => eventId,
            1 => url,
            2 => description,
            3 => tgChatIds,
            _ => throw new AvroRuntimeException($"Bad index {fieldPos}")
        };
    }

    public virtual void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                eventId = (string)fieldValue;
                break;
            case 1:
                url = (string)fieldValue;
                break;
            case 2:
                description = (string)fieldValue;
                break;
            case 3:
                tgChatIds = (IList<long>)fieldValue;
                break;
            default:
                throw new AvroRuntimeException($"Bad index {fieldPos}");
        }
    }
}