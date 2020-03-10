using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RenosFriendsList.API.Enums;

namespace RenosFriendsList.API.Entities
{
    public class Dog
    {
        public int Id { get; set; }

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

        [ForeignKey(nameof(OwnerId))]
        public Owner Owner { get; set; }

        public int OwnerId { get; set; }
    }
}
