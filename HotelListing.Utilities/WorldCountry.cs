using System.Text.Json.Serialization;

namespace HotelListing.Utilities
{
    public class WorldCountry
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}
