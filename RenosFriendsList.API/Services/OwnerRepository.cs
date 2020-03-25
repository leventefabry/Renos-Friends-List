using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RenosFriendsList.API.Data;
using RenosFriendsList.API.Entities;
using RenosFriendsList.API.ResourceParameters;

namespace RenosFriendsList.API.Services
{
    public class OwnerRepository : IDisposable, IOwnerRepository
    {
        private readonly RenosFriendsListContext _context;

        public OwnerRepository(RenosFriendsListContext context)
        {
            _context = context;
        }

        public IEnumerable<Owner> GetOwners()
        {
            return _context.Owners.Include(o => o.Dogs).OrderBy(o => o.Name).ToList();
        }

        public IEnumerable<Owner> GetOwners(OwnersResourceParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(parameters.Name) && string.IsNullOrWhiteSpace(parameters.Description))
            {
                GetOwners();
            }

            var collection = _context.Owners as IQueryable<Owner>;

            if (!string.IsNullOrWhiteSpace(parameters.Name))
            {
                var name = parameters.Name.ToLower().Trim();
                collection = collection.Where(o => o.Name.ToLower().Contains(name.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(parameters.Description))
            {
                var description = parameters.Description.ToLower().Trim();
                collection = collection.Where(o => o.Description.ToLower().Contains(description.ToLower()));
            }

            return collection.Include(o => o.Dogs).OrderBy(o => o.Name).ToList();
        }

        public IEnumerable<Owner> GetOwners(IEnumerable<int> ownerIds)
        {
            if (ownerIds == null)
            {
                throw new ArgumentNullException(nameof(ownerIds));
            }

            return _context.Owners.Where(o => ownerIds.Contains(o.Id))
                .Include(o => o.Dogs)
                .OrderBy(o => o.Name)
                .ToList();
        }

        public Owner GetOwner(int ownerId)
        {
            if (ownerId == 0)
            {
                throw new ArgumentNullException(nameof(ownerId));
            }

            return _context.Owners.Include(o => o.Dogs).FirstOrDefault(o => o.Id == ownerId);
        }

        public bool OwnerExists(int ownerId)
        {
            if (ownerId == 0)
            {
                throw new ArgumentNullException(nameof(ownerId));
            }

            return _context.Owners.Any(o => o.Id == ownerId);
        }

        public void AddOwner(Owner owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            _context.Owners.Add(owner);
            _context.SaveChanges();
        }

        public void UpdateOwner(Owner owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            _context.Owners.Update(owner);
            _context.SaveChanges();
        }

        public void DeleteOwner(Owner owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            _context.Owners.Remove(owner);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
