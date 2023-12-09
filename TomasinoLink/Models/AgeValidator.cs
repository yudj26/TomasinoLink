using System.ComponentModel.DataAnnotations;

namespace TomasinoLink.Models
{
    public class AgeValidator
    {
        public static ValidationResult ValidateLegalAge(DateTime dateOfBirth, ValidationContext context)
        {
            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

            if (age < 18)
            {
                return new ValidationResult("User must be at least 18 years old.");
            }

            return ValidationResult.Success;
        }
    }
}