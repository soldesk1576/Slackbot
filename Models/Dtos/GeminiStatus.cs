using System.Text.Json.Serialization;

namespace StatusNotifier.Models.Dtos
{
    public class GeminiStatus
    {
        [JsonPropertyName("page")]
        public Page Page { get; set; }

        [JsonPropertyName("status")]
        public Status Status { get; set; }
    }
}
