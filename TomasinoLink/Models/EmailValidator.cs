using System.ComponentModel.DataAnnotations;

namespace TomasinoLink.Models
{
    public class EmailValidator
    {
        public static ValidationResult ValidateUSTEmail(string email, ValidationContext context)
        {
            if (!email.EndsWith("@ust.edu.ph"))
            {
                return new ValidationResult("Email must be a UST email address (ends with @ust.edu.ph).");
            }

            return ValidationResult.Success;
        }
    }
}