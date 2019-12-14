using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Podrace.Model
{
    public static class JsonRacefileFormat
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Allow,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true,
            IgnoreReadOnlyProperties = true
        };

        public static async Task<Racefile> LoadAsync(string filePath, CancellationToken cancellationToken)
        {
            using (var stream = File.OpenRead(filePath))
            {
                return await ParseAsync(stream, cancellationToken);
            }
        }

        public static async Task<Racefile> ParseAsync(Stream stream, CancellationToken cancellationToken)
        {
            return await JsonSerializer.DeserializeAsync<Racefile>(
                stream,
                _options,
                cancellationToken);
        }
    }
}