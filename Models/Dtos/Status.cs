using System.Text.Json.Serialization;

namespace StatusNotifier.Models.Dtos
{
    public class Status
    {
        [JsonPropertyName("indicator")]
        public string Indicator { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
