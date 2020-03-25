using System.ComponentModel.DataAnnotations;
using RenosFriendsList.API.Models.Owner;

namespace RenosFriendsList.API.ValidationAttributes
{
    public class OwnerNameMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var owner = (OwnerForManipulationDto)validationContext.ObjectInstance;

            if (owner.Name == owner.Description)
            {
                return new ValidationResult(ErrorMessage, new[] { nameof(OwnerForManipulationDto) });
            }

            return ValidationResult.Success;
        }
    }
}
