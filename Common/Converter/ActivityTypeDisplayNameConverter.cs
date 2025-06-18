using CesiZen_Backend.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CesiZen_Backend.Common.Converter
{
    public class ActivityTypeDisplayNameConverter : JsonConverter<ActivityType>
    {
        public override ActivityType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? displayName = reader.GetString();

            foreach (ActivityType type in Enum.GetValues<ActivityType>())
            {
                DisplayAttribute? displayAttr = type.GetType()
                                      .GetMember(type.ToString())[0]
                                      .GetCustomAttribute<DisplayAttribute>();
                if (displayAttr?.Name == displayName)
                    return type;
            }

            throw new JsonException($"Type invalide: {displayName}");
        }

        public override void Write(Utf8JsonWriter writer, ActivityType value, JsonSerializerOptions options)
        {
            DisplayAttribute? displayAttr = value.GetType()
                                   .GetMember(value.ToString())[0]
                                   .GetCustomAttribute<DisplayAttribute>();
            writer.WriteStringValue(displayAttr?.Name ?? value.ToString());
        }
    }
}
