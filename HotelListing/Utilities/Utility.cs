using System.Text.Json;

namespace HotelListing.Utilities
{
    public static class Utility
    {
        public static IEnumerable<WorldCountry> Countries
        {
            get
            {
                string json = File.ReadAllText(Path.Combine("Utilities", "countries.json"));
                return JsonSerializer.Deserialize<List<WorldCountry>>(json);
            }
        }
    }
}
