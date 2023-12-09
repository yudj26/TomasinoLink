using System.ComponentModel.DataAnnotations;

namespace TomasinoLink.Models
{
    public class PasswordValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var password = (string)value;

            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return new ValidationResult("Password must be greater than 8 characters.");

            if (!password.Any(char.IsUpper))
                return new ValidationResult("First letter of the password should be capital.");

            if (!password.Any(char.IsDigit) || !password.Any(char.IsLetter))
                return new ValidationResult("Password should be alphanumeric.");

            if (!password.Any(ch => "!@#$%^&*()".Contains(ch)))
                return new ValidationResult("Password must contain a special character (@, $, !, &, etc).");

            return ValidationResult.Success;
        }
    }
}