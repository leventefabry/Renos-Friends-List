using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RenosFriendsList.API.Entities;

namespace RenosFriendsList.API.Data
{
    public class Seed
    {
        public static void SeedOwners(RenosFriendsListContext context)
        {
            if (!context.Owners.Any())
            {
                var ownersData = System.IO.File.ReadAllText("Data/OwnersSeedData.json");
                var owners = JsonConvert.DeserializeObject<List<Owner>>(ownersData);

                context.Owners.AddRange(owners);
                context.SaveChanges();
            }
        }
    }
}
