
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Datatables.AspNetCore
{
    public static class JsonExtensions
    {
        static JsonExtensions()
        {
            _jsonOptions.Converters.Add(new LongToStringConverter());
            _jsonOptions.Converters.Add(new IntToStringConverter());
            _jsonOptions.Converters.Add(new StringConverter());
        }
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };
        public static T FromJson<T>(this string json) => JsonSerializer.Deserialize<T>(json, _jsonOptions);
        public static string ToJson<T>(this T obj) =>
            JsonSerializer.Serialize(obj, _jsonOptions);
        public static void ToJson<T>(this T obj, Utf8JsonWriter writer) => JsonSerializer.Serialize(writer, obj, _jsonOptions);
    }
}
