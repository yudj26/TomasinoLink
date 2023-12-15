using System.ComponentModel.DataAnnotations;
using TomasinoLink.Models;

namespace TomasinoLink.ViewModels
{
    public class RegisterViewModel
    {
        [Required, EmailAddress, StringLength(255)]
        [CustomValidation(typeof(EmailValidator), nameof(EmailValidator.ValidateUSTEmail))]
        public string? Email { get; set; }

        [Required, DataType(DataType.Password)]
        [PasswordValidator] // Custom password validation attribute
        public string? Password { get; set; }

        [DataType(DataType.Date)]
        [CustomValidation(typeof(AgeValidator), nameof(AgeValidator.ValidateLegalAge))]
        public DateTime DateOfBirth { get; set; }

        [Required, StringLength(50)]
        public string? Gender { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
    