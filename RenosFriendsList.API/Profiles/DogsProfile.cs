using AutoMapper;
using RenosFriendsList.API.Entities;
using RenosFriendsList.API.Helpers;
using RenosFriendsList.API.Models.Dog;

namespace RenosFriendsList.API.Profiles
{
    public class DogsProfile : Profile
    {
        public DogsProfile()
        {
            CreateMap<Dog, DogDto>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()));

            CreateMap<DogForCreationDto, Dog>();
            CreateMap<DogForUpdateDto, Dog>();
            CreateMap<Dog, DogForUpdateDto>();
        }
    }
}
