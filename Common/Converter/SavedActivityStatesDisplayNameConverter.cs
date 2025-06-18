using CesiZen_Backend.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CesiZen_Backend.Common.Converter
{
    public class SavedActivityStatesDisplayNameConverter : JsonConverter<SavedActivityStates>
    {
        public override SavedActivityStates Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? displayName = reader.GetString();

            foreach (SavedActivityStates state in Enum.GetValues<SavedActivityStates>())
            {
                DisplayAttribute? displayAttr = state.GetType()
                                      .GetMember(state.ToString())[0]
                                      .GetCustomAttribute<DisplayAttribute>();
                if (displayAttr?.Name == displayName)
                    return state;
            }

            throw new JsonException($"Type invalide: {displayName}");
        }

        public override void Write(Utf8JsonWriter writer, SavedActivityStates value, JsonSerializerOptions options)
        {
            DisplayAttribute? displayAttr = value.GetType()
                                   .GetMember(value.ToString())[0]
                                   .GetCustomAttribute<DisplayAttribute>();
            writer.WriteStringValue(displayAttr?.Name ?? value.ToString());
        }
    }
}
