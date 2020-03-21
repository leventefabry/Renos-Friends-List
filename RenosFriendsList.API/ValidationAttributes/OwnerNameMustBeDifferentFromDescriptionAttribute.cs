using System.ComponentModel.DataAnnotations;
using RenosFriendsList.API.Models;

namespace RenosFriendsList.API.ValidationAttributes
{
    public class OwnerNameMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var owner = (OwnerForCreationDto) validationContext.ObjectInstance;

            if (owner.Name == owner.Description)
            {
                return new ValidationResult("The provided name should be different from the description",
                    new[] { nameof(OwnerForCreationDto) });
            }

            return ValidationResult.Success;
        }
    }
}
