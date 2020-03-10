using System;
using System.Collections.Generic;
using System.Linq;
using RenosFriendsList.API.Data;
using RenosFriendsList.API.Entities;

namespace RenosFriendsList.API.Services
{
    public class DogRepository : IDisposable, IDogRepository
    {
        private readonly RenosFriendsListContext _context;

        public DogRepository(RenosFriendsListContext context)
        {
            _context = context;
        }

        public IEnumerable<Dog> GetDogs(int ownerId)
        {
            if (ownerId == 0)
            {
                throw new ArgumentNullException(nameof(ownerId));
            }

            return _context.Dogs.Where(d => d.OwnerId == ownerId).OrderBy(d => d.Name).ToList();
        }

        public Dog GetDog(int ownerId, int dogId)
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
