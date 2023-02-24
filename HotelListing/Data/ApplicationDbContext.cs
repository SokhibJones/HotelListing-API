using HotelListing.Utilities;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Hotel> Hotels { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var countries = Utility.Countries.ToList();
            modelBuilder.Entity<Country>().HasData(Enumerable.Range(0, countries.Count - 1)
                .Select(i => new Country
                {
                    Id = i + 1,
                    Name = countries[i].Name,
                    Code = countries[i].Code
                }
            ));

            modelBuilder.Entity<Hotel>().HasData(
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
