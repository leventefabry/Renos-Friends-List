using System.Collections.Generic;
using RenosFriendsList.API.Entities;

namespace RenosFriendsList.API.Services
{
    public interface IOwnerRepository
    {
        IEnumerable<Owner> GetOwners();
        IEnumerable<Owner> GetOwners(IEnumerable<int> ownerIds);
        Owner GetOwner(int ownerId);
        bool OwnerExists(int ownerId);
        void AddOwner(Owner owner);
        void UpdateOwner(Owner owner);
        void DeleteOwner(Owner owner);
    }
}