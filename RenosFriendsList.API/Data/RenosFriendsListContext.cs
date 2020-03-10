using Microsoft.EntityFrameworkCore;
using RenosFriendsList.API.Entities;

namespace RenosFriendsList.API.Data
{
    public class RenosFriendsListContext : DbContext
    {
        public RenosFriendsListContext(DbContextOptions<RenosFriendsListContext> options) : base(options)
        {
        }

        public DbSet<Dog> Dogs { get; set; }
        public DbSet<Owner> Owners { get; set; }
    }
}
