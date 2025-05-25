using System.ComponentModel.DataAnnotations;

namespace RideSharingApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public bool IsDriver { get; set; }
    }
}