using System.ComponentModel.DataAnnotations;

namespace BookAPI.Dtos.Validations
{
    public class NotDefaultDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateOnly dateValue && dateValue == DateOnly.MinValue)
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
