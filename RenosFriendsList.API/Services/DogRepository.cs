﻿using System;
using System.Collections.Generic;
using System.Linq;
using RenosFriendsList.API.Data;
using RenosFriendsList.API.Entities;
using RenosFriendsList.API.Helpers;
using RenosFriendsList.API.Models.Dog;
using RenosFriendsList.API.ResourceParameters;
using RenosFriendsList.API.Services.PropertyMapping;

namespace RenosFriendsList.API.Services
{
    public class DogRepository : IDisposable, IDogRepository
    {
        private readonly RenosFriendsListContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public DogRepository(RenosFriendsListContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public IEnumerable<Dog> GetDogs(int ownerId)
        {
            if (ownerId == 0)
            {
                throw new ArgumentNullException(nameof(ownerId));
            }

            return _context.Dogs.Where(d => d.OwnerId == ownerId).OrderBy(d => d.Name).ToList();
        }

        public PagedList<Dog> GetAllDogs(DogsResourceParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var collection = _context.Dogs as IQueryable<Dog>;

            if (!string.IsNullOrWhiteSpace(parameters.Name))
            {
                var name = parameters.Name.ToLower().Trim();
                collection = collection.Where(d => d.Name.ToLower().Contains(name));
            }

            if (parameters.RenoLikesIt.HasValue)
            {
                collection = collection.Where(d => d.RenoLikesIt == parameters.RenoLikesIt.Value);
            }

            if (parameters.BodyType.HasValue)
            {
                collection = collection.Where(d => d.BodyType == parameters.BodyType);
            }

            if (parameters.Gender.HasValue)
            {
                collection = collection.Where(d => d.Gender == parameters.Gender);
            }

            if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
            {
                var dogPropertyMappingDictionary = _propertyMappingService.GetPropertyMapping<DogDto, Dog>();
                collection = collection.ApplySort(parameters.OrderBy, dogPropertyMappingDictionary);
            }

            return PagedList<Dog>.Create(
                collection,
                    parameters.PageNumber,
                    parameters.PageSize);
        }

        public Dog GetDogByOwner(int ownerId, int dogId)
        {
            if (ownerId == 0)
            {
                throw new ArgumentNullException(nameof(ownerId));
            }

            if (dogId == 0)
            {
                throw new ArgumentNullException(nameof(dogId));
            }

            return _context.Dogs.FirstOrDefault(d => d.OwnerId == ownerId && d.Id == dogId);
        }

        public Dog GetDog(int dogId)
        {
            if (dogId == 0)
            {
                throw new ArgumentNullException(nameof(dogId));
            }

            return _context.Dogs.FirstOrDefault(d => d.Id == dogId);
        }

        public void AddDog(int ownerId, Dog dog)
        {
            if (ownerId == 0)
            {
                throw new ArgumentNullException(nameof(ownerId));
            }

            if (dog == null)
            {
                throw new ArgumentNullException(nameof(dog));
            }

            dog.OwnerId = ownerId;
            _context.Dogs.Add(dog);
            _context.SaveChanges();
        }

        public void UpdateDog(Dog dog)
        {
            if (dog == null)
            {
                throw new ArgumentNullException(nameof(dog));
            }

            _context.Dogs.Update(dog);
            _context.SaveChanges();
        }

        public void DeleteDog(Dog dog)
        {
            _context.Dogs.Remove(dog);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
