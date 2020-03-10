using System.Collections.Generic;
using RenosFriendsList.API.Entities;

namespace RenosFriendsList.API.Services
{
    public interface IDogRepository
    {
        IEnumerable<Dog> GetDogs(int ownerId);
        Dog GetDog(int ownerId, int dogId);
        void AddDog(int ownerId, Dog dog);
        void UpdateDog(Dog dog);
        void DeleteDog(Dog dog);
    }
}