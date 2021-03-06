﻿using System.Collections.Generic;
using RenosFriendsList.API.Entities;
using RenosFriendsList.API.Helpers;
using RenosFriendsList.API.ResourceParameters;

namespace RenosFriendsList.API.Services
{
    public interface IDogRepository
    {
        IEnumerable<Dog> GetDogs(int ownerId);
        PagedList<Dog> GetAllDogs(DogsResourceParameters parameters);
        Dog GetDogByOwner(int ownerId, int dogId);
        Dog GetDog(int dogId);
        void AddDog(int ownerId, Dog dog);
        void UpdateDog(Dog dog);
        void DeleteDog(Dog dog);
    }
}