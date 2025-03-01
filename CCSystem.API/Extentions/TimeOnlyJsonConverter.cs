using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CCSystem.API.Extentions
{
    public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
    {
        private const string Format = "HH:mm:ss";

        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var timeString = reader.GetString();
            if (TimeOnly.TryParseExact(timeString, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
            {
                return time;
            }
            throw new JsonException($"Unable to convert \"{timeString}\" to TimeOnly.");
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }
}
