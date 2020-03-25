using System.Collections.Generic;
using RenosFriendsList.API.Models.Dog;

namespace RenosFriendsList.API.Models.Owner
{
    public class OwnerForCreationDto : OwnerForManipulationDto
    {
        public ICollection<DogForCreationDto> Dogs { get; set; } = new List<DogForCreationDto>();
    }
}
