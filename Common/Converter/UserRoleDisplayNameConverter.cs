using CesiZen_Backend.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CesiZen_Backend.Common.Converter
{
    public class UserRoleDisplayNameConverter : JsonConverter<UserRole>
    {
        public override UserRole Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? displayName = reader.GetString();

            foreach (UserRole role in Enum.GetValues<UserRole>())
            {
                DisplayAttribute? displayAttr = role.GetType()
                                      .GetMember(role.ToString())[0]
                                      .GetCustomAttribute<DisplayAttribute>();
                if (displayAttr?.Name == displayName)
                    return role;
            }

            throw new JsonException($"Rôle invalide: {displayName}");
        }

        public override void Write(Utf8JsonWriter writer, UserRole value, JsonSerializerOptions options)
        {
            DisplayAttribute? displayAttr = value.GetType()
                                   .GetMember(value.ToString())[0]
                                   .GetCustomAttribute<DisplayAttribute>();
            writer.WriteStringValue(displayAttr?.Name ?? value.ToString());
        }
    }
}
