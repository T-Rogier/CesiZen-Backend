using CesiZen_Backend.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

    public class DisplayNameEnumModelBinder<T> : IModelBinder where T : struct, Enum
    {
        public Task BindModelAsync(ModelBindingContext ctx)
        {
            var val = ctx.ValueProvider.GetValue(ctx.FieldName).FirstValue;
            if (string.IsNullOrEmpty(val)) return Task.CompletedTask;
            foreach (T enumVal in Enum.GetValues<T>())
            {
                var disp = enumVal.GetType()
                          .GetMember(enumVal.ToString())[0]
                          .GetCustomAttribute<DisplayAttribute>()?.Name;
                if (disp == val)
                {
                    ctx.Result = ModelBindingResult.Success(enumVal);
                    return Task.CompletedTask;
                }
            }
            ctx.ModelState.TryAddModelError(ctx.FieldName, $"Type invalide: {val}");
            return Task.CompletedTask;
        }
    }
}
