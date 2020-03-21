using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RenosFriendsList.API.ValidationAttributes;

namespace RenosFriendsList.API.Models
{
    [OwnerNameMustBeDifferentFromDescription]
    public class OwnerForCreationDto
    {
        [Required(ErrorMessage = "You should fill out a name.")]
        [MaxLength(50, ErrorMessage = "The name shouldn't have more than 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You should fill out a description.")]
        [MaxLength(1500, ErrorMessage = "The name shouldn't have more than 1500 characters.")]
        public string Description { get; set; }

        public ICollection<DogForCreationDto> Dogs { get; set; } = new List<DogForCreationDto>();
    }
}
