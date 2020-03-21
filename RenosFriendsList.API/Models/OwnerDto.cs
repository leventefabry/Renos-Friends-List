using System.Collections.Generic;

namespace RenosFriendsList.API.Models
{
    public class OwnerDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<DogDto> Dogs { get; set; }
}
}
