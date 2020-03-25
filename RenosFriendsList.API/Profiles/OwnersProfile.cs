using AutoMapper;
using RenosFriendsList.API.Entities;
using RenosFriendsList.API.Models.Owner;

namespace RenosFriendsList.API.Profiles
{
    public class OwnersProfile : Profile
    {
        public OwnersProfile()
        {
            CreateMap<Owner, OwnerDto>();
            CreateMap<OwnerForCreationDto, Owner>();
            CreateMap<OwnerForUpdateDto, Owner>();
            CreateMap<Owner, OwnerForUpdateDto>();
        }
    }
}
