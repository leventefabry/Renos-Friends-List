using System.ComponentModel.DataAnnotations;
using RenosFriendsList.API.ValidationAttributes;

namespace RenosFriendsList.API.Models.Owner
{
    [OwnerNameMustBeDifferentFromDescription(ErrorMessage = "The provided name should be different from the description")]
    public abstract class OwnerForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a name.")]
        [MaxLength(50, ErrorMessage = "The name shouldn't have more than 50 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You should fill out a description.")]
        [MaxLength(1500, ErrorMessage = "The description shouldn't have more than 1500 characters.")]
        public string Description { get; set; }
    }
}
