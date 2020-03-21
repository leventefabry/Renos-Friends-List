using System;
using System.ComponentModel.DataAnnotations;
using RenosFriendsList.API.Enums;

namespace RenosFriendsList.API.Models
{
    public class DogForCreationDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public bool RenoLikesIt { get; set; }

        [Required]
        public BodySizeEnum BodyType { get; set; }

        [Required]
        public GenderEnum Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
}
