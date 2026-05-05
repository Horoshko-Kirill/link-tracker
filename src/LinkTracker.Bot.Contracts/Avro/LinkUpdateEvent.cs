using Avro;
using Avro.Specific;

namespace LinkTracker.Bot.Contracts.Avro;

public class LinkUpdateEvent : ISpecificRecord
{
    public static Schema _SCHEMA = Schema.Parse(
        """
        {
            "type": "record",
            "name": "LinkUpdateEvent",
            "namespace": "LinkTracker.Bot.Contracts.Avro",
            "fields": [
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
    public string url { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public IList<long> tgChatIds { get; set; } = new List<long>();

    public virtual object Get(int fieldPos)
    {
        return fieldPos switch
        {
            0 => url,
            1 => description,
            2 => tgChatIds,
            _ => throw new AvroRuntimeException($"Bad index {fieldPos}")
        };
    }

    public virtual void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0:
                url = (string)fieldValue;
                break;
            case 1:
                description = (string)fieldValue;
                break;
            case 2:
                tgChatIds = (IList<long>)fieldValue;
                break;
            default:
                throw new AvroRuntimeException($"Bad index {fieldPos}");
        }
    }
}