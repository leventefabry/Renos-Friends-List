using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using RenosFriendsList.API.Enums;
using RenosFriendsList.API.ValidationAttributes;

namespace RenosFriendsList.API.Models
{
    public abstract class DogForManipulationDto
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
