using System.Text.Json.Serialization;

namespace StatusNotifier.Models.Dtos
{
    public class Page
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        [JsonPropertyName("time_zone")]
        public string TimeZone { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
