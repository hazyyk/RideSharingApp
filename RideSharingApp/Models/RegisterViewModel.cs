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

        // Driver-specific fields
        public string? LicenseNumber { get; set; }
        public string? VehicleModel { get; set; }
        public string? VehicleType { get; set; }
        public string? LicensePlate { get; set; }
        public int? Capacity { get; set; }
    }
}