using HotelListing.Data;
using HotelListing.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Configurations.Entities
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            var countries = Utility.Countries.ToList();
            builder.HasData(Enumerable.Range(0, countries.Count - 1)
                .Select(i => new Country
                {
                    Id = i + 1,
                    Name = countries[i].Name,
                    Code = countries[i].Code
                }
            ));
        }
    }
}
