using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Data.Configurations.Entities
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "The Beverly Hills Hotel",
                    Address = "Beverly Hills, California",
                    Rating = 4.8,
                    CountryId = 230
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Fairmont Le Château Frontenac",
                    Address = "Quebec City, Quebec, Canada",
                    Rating = 4.7,
                    CountryId = 39
                },
                new Hotel
                {
                    Id = 3,
                    Name = "Caesars Palace",
                    Address = "Nevada, United States",
                    Rating = 4.9,
                    CountryId = 230
                });
        }
    }
}
