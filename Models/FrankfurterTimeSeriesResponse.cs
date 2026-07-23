using System.Text.Json.Serialization;

namespace CurrencyManagement.Models
{
    public class FrankfurterTimeSeriesResponse
    {
        [JsonPropertyName("base")]
        public string Base { get; set; } = string.Empty;

        [JsonPropertyName("rates")]
        public Dictionary<string, Dictionary<string, decimal>> Rates
        {
            get;
            set;
        } = new();
    }
}