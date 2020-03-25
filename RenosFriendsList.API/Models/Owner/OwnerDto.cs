using System.Collections.Generic;
using RenosFriendsList.API.Models.Dog;

namespace RenosFriendsList.API.Models.Owner
{
    public class OwnerDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<DogDto> Dogs { get; set; }
}
}
