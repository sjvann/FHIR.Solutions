using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Abstractions;
using Fhir.TypeFramework.DataTypes;

namespace Fhir.TypeFramework.Serialization;

/// <summary>
/// System.Text.Json cannot deserialize into <see cref="IExtension"/>; FHIR wire JSON uses concrete <see cref="Extension"/> elements.
/// </summary>
public sealed class FhirExtensionListJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
            return false;

        var def = typeToConvert.GetGenericTypeDefinition();
        if (def != typeof(List<>) && def != typeof(IList<>))
            return false;

        return typeToConvert.GetGenericArguments()[0] == typeof(IExtension);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var listKind = typeToConvert.GetGenericTypeDefinition();
        return listKind == typeof(IList<>)
            ? new IListOfExtensionConverter()
            : new ListOfExtensionConverter();
    }

    sealed class ListOfExtensionConverter : JsonConverter<List<IExtension>?>
    {
        public override List<IExtension>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            var concrete = JsonSerializer.Deserialize<List<Extension>>(ref reader, options);
            if (concrete == null)
                return null;

            var result = new List<IExtension>(concrete.Count);
            foreach (var e in concrete)
                result.Add(e);
            return result;
        }

        public override void Write(Utf8JsonWriter writer, List<IExtension>? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            WriteExtensions(writer, value, options);
        }
    }

    sealed class IListOfExtensionConverter : JsonConverter<IList<IExtension>?>
    {
        public override IList<IExtension>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            var concrete = JsonSerializer.Deserialize<List<Extension>>(ref reader, options);
            if (concrete == null)
                return null;

            var result = new List<IExtension>(concrete.Count);
            foreach (var e in concrete)
                result.Add(e);
            return result;
        }

        public override void Write(Utf8JsonWriter writer, IList<IExtension>? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            WriteExtensions(writer, value, options);
        }
    }

    static void WriteExtensions(Utf8JsonWriter writer, IEnumerable<IExtension> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var ext in value)
        {
            if (ext is Extension concrete)
                JsonSerializer.Serialize(writer, concrete, options);
            else
                JsonSerializer.Serialize(writer, ext, ext.GetType(), options);
        }

        writer.WriteEndArray();
    }
}
