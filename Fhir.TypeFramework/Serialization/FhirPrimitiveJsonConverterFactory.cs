using System.Text.Json;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.Serialization;

/// <summary>
/// 將 <see cref="PrimitiveType{T}"/> 以 FHIR JSON 慣用之「純量」形式讀寫（例如 <c>coding.system</c> 為字串，而非巢狀物件）。
/// 仍相容舊版 <c>{ "value": "..." }</c> 物件形式以便反序列化。
/// </summary>
public sealed class FhirPrimitiveJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => GetPrimitiveType(typeToConvert) != null;

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var prim = GetPrimitiveType(typeToConvert)!;
        var valueType = prim.GetGenericArguments()[0];
        var converterType = typeof(FhirPrimitiveJsonConverter<,>).MakeGenericType(typeToConvert, valueType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private static Type? GetPrimitiveType(Type type)
    {
        for (var t = type; t != null; t = t.BaseType)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(PrimitiveType<>))
                return t;
        }

        return null;
    }

    private sealed class FhirPrimitiveJsonConverter<T, TValue> : JsonConverter<T> where T : PrimitiveType<TValue>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var instance = (T)Activator.CreateInstance(typeToConvert)!;

            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return instance;
                case JsonTokenType.String:
                    instance.StringValue = reader.GetString();
                    return instance;
                case JsonTokenType.True:
                case JsonTokenType.False:
                    instance.StringValue = reader.GetBoolean() ? "true" : "false";
                    return instance;
                case JsonTokenType.Number:
                    if (reader.TryGetInt32(out var i))
                        instance.StringValue = i.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    else if (reader.TryGetInt64(out var l))
                        instance.StringValue = l.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    else if (reader.TryGetDecimal(out var d))
                        instance.StringValue = d.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    else
                        instance.StringValue = reader.GetDouble().ToString(System.Globalization.CultureInfo.InvariantCulture);
                    return instance;
                case JsonTokenType.StartObject:
                    using (var doc = JsonDocument.ParseValue(ref reader))
                    {
                        var root = doc.RootElement;
                        if (root.TryGetProperty("value", out var valueEl))
                            instance.StringValue = ValueElementToString(valueEl);
                        return instance;
                    }
                default:
                    throw new JsonException($"Unexpected token for primitive: {reader.TokenType}");
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value.IsNull)
            {
                writer.WriteNullValue();
                return;
            }

            var v = value.Value;
            switch (v)
            {
                case bool b:
                    writer.WriteBooleanValue(b);
                    return;
                case int bi:
                    writer.WriteNumberValue(bi);
                    return;
                case long bl:
                    writer.WriteNumberValue(bl);
                    return;
                case decimal bd:
                    writer.WriteNumberValue(bd);
                    return;
                case double bf:
                    writer.WriteNumberValue(bf);
                    return;
            }

            writer.WriteStringValue(value.StringValue);
        }

        private static string? ValueElementToString(JsonElement el) =>
            el.ValueKind switch
            {
                JsonValueKind.String => el.GetString(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Number => el.GetRawText(),
                _ => el.GetRawText()
            };
    }
}
