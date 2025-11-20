using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Replay.Utils;

public class SerializableDateJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(SerializableDate);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        JObject jsonObject = JObject.Load(reader);
        var retVal = existingValue as SerializableDate ?? new SerializableDate();

        if (jsonObject["month"] != null)
        {
            string monthString = jsonObject["month"].ToString();
            if (Enum.TryParse<SerializableDate.Month>(monthString, true, out var month))
                retVal.month = month;
        }

        if (jsonObject["day"] != null)
            retVal.day = jsonObject["day"].Value<int>();

        if (jsonObject["year"] != null)
            retVal.year = jsonObject["year"].Value<int>();

        return retVal;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        SerializableDate date = (SerializableDate)value;

        writer.WriteStartObject();
        writer.WritePropertyName("month");
        writer.WriteValue(date.month.ToString());
        writer.WritePropertyName("day");
        writer.WriteValue(date.day);
        writer.WritePropertyName("year");
        writer.WriteValue(date.year);
        writer.WriteEndObject();
    }
}