using System.Text.Json.Serialization;
using System.Text.Json;

public class JsonNumberConverter : JsonConverter<object>
{
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt64(out long longValue)) return longValue; // Convert integers
            if (reader.TryGetDouble(out double doubleValue)) return doubleValue; // Convert doubles
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            string str = reader.GetString();
            if (double.TryParse(str, out double result)) return result; // Convert string to double
        }
        return JsonSerializer.Deserialize<object>(ref reader, options); // Default handling
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options); // Default serialization
    }
}
