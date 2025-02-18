using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class MeowFactResponse
    {
        [JsonPropertyName("data")]
        public string[] Data { get; set; } = [];
    }
}
