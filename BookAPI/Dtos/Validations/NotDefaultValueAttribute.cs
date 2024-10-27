using System.ComponentModel.DataAnnotations;

namespace BookAPI.Dtos.Validations
{
    public class NotDefaultValueAttribute : ValidationAttribute
    {
        private readonly string _defaultValue;

        public NotDefaultValueAttribute(string defaultValue)
        {
            _defaultValue = defaultValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (value is string stringValue && stringValue == _defaultValue)
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
