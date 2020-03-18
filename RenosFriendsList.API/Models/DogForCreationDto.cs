using System;
using RenosFriendsList.API.Enums;

namespace RenosFriendsList.API.Models
{
    public class DogForCreationDto
    {
        public string Name { get; set; }

        public bool RenoLikesIt { get; set; }

        public BodySizeEnum BodyType { get; set; }

        public GenderEnum Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}
