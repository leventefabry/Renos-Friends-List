using AutoMapper;
using RenosFriendsList.API.Entities;
using RenosFriendsList.API.Models;

namespace RenosFriendsList.API.Profiles
{
    public class OwnersProfile : Profile
    {
        public OwnersProfile()
        {
            CreateMap<Owner, OwnerDto>();
        }
    }
}
