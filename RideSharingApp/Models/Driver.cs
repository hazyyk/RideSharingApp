
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RideSharingApp.Models
{
    public class Driver
    {
        [Key]
        public string DriverID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string? VehicleID { get; set; }
        public virtual Vehicle? Vehicle { get; set; }

        public string IdentityUserId { get; set; } = string.Empty;
        public virtual ICollection<RideBooking> RideBookings { get; set; } = new List<RideBooking>();

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}