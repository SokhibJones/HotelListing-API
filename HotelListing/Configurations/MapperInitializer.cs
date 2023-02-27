using AutoMapper;
using HotelListing.Data;
using HotelListing.DTOs;

namespace HotelListing.Configurations
{
    public class MapperInitializer : Profile
    {
        public MapperInitializer()
        {
            CreateMap<Country, CountryDTO>().ReverseMap();
            CreateMap<Country, CreateCountryDTO>().ReverseMap();
            CreateMap<WorldCountryDTO, Country>().ReverseMap();
            CreateMap<Hotel, HotelDTO>().ReverseMap();
            CreateMap<Hotel, CreateHotelDTO>().ReverseMap();
            CreateMap<UserRegisterDTO, ApiUser>().ReverseMap();
        }
    }
}
