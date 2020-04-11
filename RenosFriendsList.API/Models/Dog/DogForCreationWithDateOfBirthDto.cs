using System;

namespace RenosFriendsList.API.Models.Dog
{
    public class DogForCreationWithDateOfBirthDto : DogForManipulationDto
    {
        public DateTime? DateOfBirth { get; set; }
    }
}
