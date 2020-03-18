using System.Collections.Generic;

namespace RenosFriendsList.API.Models
{
    public class OwnerForCreationDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<DogForCreationDto> Dogs { get; set; } = new List<DogForCreationDto>();
    }
}
