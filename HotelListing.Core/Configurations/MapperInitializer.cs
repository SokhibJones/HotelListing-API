using AutoMapper;
using HotelListing.Core.DTOs;
using HotelListing.Data;

namespace HotelListing.Core.Configurations
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
            CreateMap<CoutryHotelDTO, Hotel>().ReverseMap();
            CreateMap<UpsertHotelDTO, Hotel>().ReverseMap();
            CreateMap<Country, UpdateCountryDTO>().ReverseMap();
            CreateMap<UserRegisterDTO, ApiUser>().ReverseMap();
        }
    }
}
