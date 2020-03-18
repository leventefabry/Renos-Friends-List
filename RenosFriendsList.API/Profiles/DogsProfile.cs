using AutoMapper;
using RenosFriendsList.API.Entities;
using RenosFriendsList.API.Models;
using RenosFriendsList.API.Helpers;

namespace RenosFriendsList.API.Profiles
{
    public class DogsProfile : Profile
    {
        public DogsProfile()
        {
            CreateMap<Dog, DogDto>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()));

            CreateMap<DogForCreationDto, Dog>();
        }
    }
}
