using System.ComponentModel.DataAnnotations;

namespace BookAPI.Dtos.Validations
{
    public class NotPlaceholderStringAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is List<string> stringList)
            {
                if (stringList.All(s => s == "string"))
                {
                    return new ValidationResult($"The {validationContext.DisplayName} field must not contain only placeholder values.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
